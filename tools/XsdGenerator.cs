using DG.Epub.CodeGeneration.Models.Schema;
using System.Xml.Linq;

namespace DG.Epub.CodeGeneration;
public class XsdGenerator
{
    private static readonly XNamespace xs = "http://www.w3.org/2001/XMLSchema";
    private static readonly XNamespace tns = "http://tempuri.org/PurchaseOrderSchema.xsd";

    public static XDocument Test()
    {
        // Define namespaces
        XNamespace xs = "http://www.w3.org/2001/XMLSchema";

        // Create schema root element with appropriate attributes
        var schema = new XElement(xs + "schema",
            new XAttribute(XNamespace.Xmlns + "xs", xs),
            new XAttribute(XNamespace.Xmlns + "tns", tns),
            new XAttribute("targetNamespace", tns),
            new XAttribute("elementFormDefault", "qualified")
        );

        schema.Add(new XElement(xs + "attributeGroup",
            new XAttribute("name", "RemoveXmlDeclaration")
        ));

        var purchaseOrderType = new XElement(xs + "complexType",
            new XAttribute("name", "PurchaseOrderType"),
            new XElement(xs + "all",
                new XElement(xs + "element",
                    new XAttribute("name", "ShipTo"),
                    new XAttribute("type", "tns:USAddress"),
                    new XAttribute("minOccurs", "1"),
                    new XAttribute("nillable", "false")
                ),
                new XElement(xs + "element",
                    new XAttribute("name", "BillTo"),
                    new XAttribute("type", "tns:USAddress"),
                    new XAttribute("minOccurs", "0"),
                    new XAttribute("nillable", "true")
                )
            ),
            new XElement(xs + "attribute",
                new XAttribute("name", "OrderDate"),
                new XAttribute("type", "xs:date")
            ),
            new XElement(xs + "attributeGroup",
                new XAttribute("ref", "tns:RemoveXmlDeclaration")
            )
        );
        schema.Add(purchaseOrderType);

        var usAddressType = new XElement(xs + "complexType",
            new XAttribute("name", "USAddress"),
            new XElement(xs + "all",
                new XElement(xs + "element",
                    new XAttribute("name", "name"),
                    new XAttribute("type", "xs:string")
                ),
                new XElement(xs + "element",
                    new XAttribute("name", "street"),
                    new XAttribute("type", "xs:string")
                ),
                new XElement(xs + "element",
                    new XAttribute("name", "city"),
                    new XAttribute("type", "xs:string")
                ),
                new XElement(xs + "element",
                    new XAttribute("name", "state"),
                    new XAttribute("type", "xs:string")
                ),
                new XElement(xs + "element",
                    new XAttribute("name", "zip"),
                    new XAttribute("type", "xs:integer")
                )
            ),
            new XElement(xs + "attribute",
                new XAttribute("name", "country"),
                new XAttribute("type", "xs:NMTOKEN"),
                new XAttribute("fixed", "US")
            )
        );
        schema.Add(usAddressType);

        return new XDocument(null, schema);
    }

    public static XDocument GenerateXsd(Dictionary<string, BaseSchema> schemas)
    {
        // Create schema root element with appropriate attributes
        var schemaElement = new XElement(xs + "schema",
            new XAttribute(XNamespace.Xmlns + "xs", xs),
            new XAttribute(XNamespace.Xmlns + "tns", tns),
            new XAttribute("targetNamespace", tns),
            new XAttribute("elementFormDefault", "qualified")
        );

        schemaElement.Add(new XElement(xs + "attributeGroup",
            new XAttribute("name", "RemoveXmlDeclaration")
        ));

        foreach (var (name, schema) in schemas)
        {
            XElement element = null;
            switch (schema)
            {
                case ObjectSchema objectSchema:
                    element = CreateElementForObjectSchema(name, objectSchema);
                    break;
                case StringSchema stringSchema:
                    element = CreateElementForEnumStringSchema(name, stringSchema);
                    break;
                default:
                    break;
            }

            if (element is not null)
            {
                schemaElement.Add(element);
            }
        }

        return new XDocument(null, schemaElement);
    }

    private static XElement CreateElementForEnumStringSchema(string name, StringSchema stringSchema)
    {
        if (stringSchema.Enum == null || stringSchema.Enum.Count == 0)
        {
            return null;
        }
        if (!stringSchema.TryGetXmlName(out string xmlName))
        {
            xmlName = name;
        }
        var enumElement = new XElement(xs + "restriction",
            new XAttribute("base", "xs:string")
        );
        var simpleType = new XElement(xs + "simpleType", new XAttribute("name", xmlName), enumElement);
        foreach (var enumValue in stringSchema.Enum)
        {
            enumElement.Add(new XElement(xs + "enumeration",
                new XAttribute("value", enumValue)
            ));
        }

        return simpleType;
    }

    private static XElement CreateElementForObjectSchema(string name, ObjectSchema objectSchema)
    {
        var complexType = new XElement(xs + "complexType", new XAttribute("name", name));
        AddDescriptionFromSchema(complexType, objectSchema);

        if (objectSchema.Properties != null)
        {
            var sequence = new XElement(xs + "sequence");
            foreach (var (propName, propSchema) in objectSchema.Properties)
            {
                if (!propSchema.TryGetXmlName(out string xmlName))
                {
                    xmlName = propName;
                }
                var prop = new XElement(xs + "element",
                    new XAttribute("name", xmlName)
                );
                DecorateProperty(prop, propSchema);
                sequence.Add(prop);
            }
            complexType.Add(sequence);
        }

        if (objectSchema.Attributes != null)
        {
            foreach (var (attrName, attrSchema) in objectSchema.Attributes)
            {
                if (!attrSchema.TryGetXmlName(out string xmlName))
                {
                    xmlName = attrName;
                }
                var attr = new XElement(xs + "attribute",
                    new XAttribute("name", xmlName)
                );
                DecorateAttribute(attr, attrSchema);

                complexType.Add(attr);
            }
        }

        return complexType;
    }

    private static void DecorateProperty(XElement propertyElement, BaseSchema schema)
    {
        AddDescriptionFromSchema(propertyElement, schema);

        var typeAttribute = GetTypeAttributeForSchema(schema);
        if (typeAttribute != null)
        {
            propertyElement.Add(typeAttribute);
        }

        propertyElement.Add(new XAttribute("minOccurs", "0"));
    }

    private static void DecorateAttribute(XElement attributeElement, BaseSchema schema)
    {
        AddDescriptionFromSchema(attributeElement, schema);
        attributeElement.Add(GetTypeAttributeForSchema(schema));

        if (schema is StringSchema stringSchema)
        {
            if (!string.IsNullOrEmpty(stringSchema.Default))
            {
                attributeElement.Add(new XAttribute("default", stringSchema.Default));
            }
            if (!string.IsNullOrEmpty(stringSchema.Fixed))
            {
                attributeElement.Add(new XAttribute("fixed", stringSchema.Fixed));
            }
        }
    }

    private static XObject GetTypeAttributeForSchema(BaseSchema schema)
    {
        switch (schema)
        {
            case ArraySchema arraySchema:
                return GetArrayElement(arraySchema);
            case RefSchema refSchema:
                return new XAttribute("type", "tns:" + refSchema.Ref.TrimStart('#'));
            case StringSchema stringSchema:
            default:
                return new XAttribute("type", "xs:string");
        }
    }

    private static XElement GetArrayElement(ArraySchema arraySchema)
    {
        XElement childElement = null;
        switch (arraySchema.Items)
        {
            case ObjectSchema objectSchema:
                childElement = CreateElementForObjectSchema(Guid.NewGuid().ToString(), objectSchema);
                break;
            case RefSchema refSchema:
                childElement = new XElement(xs + "element",
                    new XAttribute("name", refSchema.Ref.TrimStart('#')),
                    new XAttribute("type", "tns:" + refSchema.Ref.TrimStart('#'))
                );
                break;
            default:
                childElement = new XElement(xs + "element",
                    new XAttribute("name", "asdfasdfasdf"),
                    GetTypeAttributeForSchema(arraySchema.Items)
                );
                break;
        }
        childElement.Add(
            new XAttribute("minOccurs", "0"),
            new XAttribute("maxOccurs", "unbounded")
        );
        return new XElement(xs + "complexType",
            new XElement(xs + "sequence",
                childElement
            )
        );
    }

    private static void AddDescriptionFromSchema(XContainer element, BaseSchema schema)
    {
        if (string.IsNullOrEmpty(schema.Description))
        {
            return;
        }

        var descriptionElement = new XElement(xs + "annotation",
            new XElement(xs + "documentation",
                new XAttribute(XNamespace.Xml + "lang", "en"),
                schema.Description
            )
        );
        element.Add(descriptionElement);
    }
}
