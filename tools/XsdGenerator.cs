using DG.Epub.CodeGeneration.Models.Schema;
using System.Xml.Linq;

namespace DG.Epub.CodeGeneration;
public class XsdGenerator
{
    public static XDocument Test()
    {
        // Define namespaces
        XNamespace xs = "http://www.w3.org/2001/XMLSchema";
        XNamespace tns = "http://tempuri.org/PurchaseOrderSchema.xsd";

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
        XNamespace xs = "http://www.w3.org/2001/XMLSchema";
        XNamespace tns = "http://tempuri.org/PurchaseOrderSchema.xsd";

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
            if (schema is not ObjectSchema objectSchema)
            {
                continue;
            }
            var complexType = new XElement(xs + "complexType", new XAttribute("name", name),
                new XElement(xs + "annotation",
                    new XElement(xs + "documentation",
                        new XAttribute(XNamespace.Xml + "lang", "en"),
                        "This is a complex type"
                    )
                )
            );

            if (objectSchema.Properties != null)
            {
                var sequence = new XElement(xs + "all");
                foreach (var (propName, propSchema) in objectSchema.Properties)
                {
                    if (!propSchema.TryGetXmlName(out string xmlName))
                    {
                        xmlName = propName;
                    }
                    sequence.Add(new XElement(xs + "element",
                        new XAttribute("name", xmlName),
                        new XAttribute("type", "xs:string"),
                        new XAttribute("minOccurs", "0")));
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
                        new XAttribute("name", xmlName),
                        new XAttribute("type", "xs:string")
                    );
                    if (attrSchema is StringSchema stringSchema)
                    {
                        if (!string.IsNullOrEmpty(stringSchema.Default))
                        {
                            attr.Add(new XAttribute("default", stringSchema.Default));
                        }
                        if (!string.IsNullOrEmpty(stringSchema.Fixed))
                        {
                            attr.Add(new XAttribute("fixed", stringSchema.Fixed));
                        }
                    }

                    complexType.Add(attr);
                }
            }

            schemaElement.Add(complexType);
        }

        return new XDocument(null, schemaElement);
    }
}
