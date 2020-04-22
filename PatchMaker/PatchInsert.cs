﻿using System;
using System.Linq;
using System.Xml.Linq;

namespace PatchMaker
{

    public class PatchInsert : BasePatch
    {
        public string XPathForParent { get; }
        public string XPathForOrder { get; }
        public ElementInsertPosition Position { get; }
        public XElement NewElement { get; }

        public PatchInsert(string xPathForParent, ElementInsertPosition position, string xPathForOrder, XElement newElement)
        {
            if(string.IsNullOrWhiteSpace(xPathForParent))
            {
                throw new ArgumentNullException(nameof(xPathForParent));
            }
            XPathForParent = xPathForParent;

            Position = position;

            if(string.IsNullOrWhiteSpace(xPathForOrder))
            {
                throw new ArgumentNullException(nameof(xPathForOrder));
            }
            XPathForOrder = xPathForOrder;

            if(newElement == null)
            {
                throw new ArgumentNullException(nameof(newElement));
            }
            NewElement = newElement;
        }

        public override void ApplyPatchElement(XDocument sourceXml, XDocument patchXml)
        {
            // select element
            var targetElement = sourceXml.SafeXPathSelectElement(XPathForParent);

            if (targetElement == null)
            {
                throw new PatchException(nameof(PatchInsert), XPathForParent, nameof(XPathForParent));
            }

            // Add patch:{Position}="{XPathForOrder}" attribute
            var newNode = new XElement(NewElement);
            newNode.Add(new XAttribute(Namespaces.Patch + Position.ToString().ToLower(), XPathForOrder));

            // append PatchXml element as child
            var currentPatchNode = base.CopyAncestorsAndSelf(targetElement, patchXml.Root);
            currentPatchNode.Add(newNode);
        }

        public override string ToString()
        {
            return $"INSERT: {Position} {XPathForOrder}";
        }
    }

}