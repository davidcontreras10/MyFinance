using System;
using System.Collections.Generic;
using System.Linq;

namespace StUtilities
{
    public interface IGenericSelectionElementAsociable
    {
        object GetId();
        string GetShowText();
        object GetElement();
    }

    public class GenericSelectionElement:IGenericSelectionElementAsociable
    {

        #region Constructors

        public override string ToString()
        {
            return ShowText;
        }

        public GenericSelectionElement(){}

        public GenericSelectionElement(object value)
        {
            if (value == null)
                throw new Exception("Not null values are accepted");
            Id = value;
            ShowText = value.ToString();
            Element = value;
        }


        #endregion

        public object Id { set; get; }
        public string ShowText { set; get; }
        public object Element { set; get; }

        public static List<GenericSelectionElement> GetGenericSelectionElementMatches(
            List<GenericSelectionElement> list, string text)
        {
            if (list == null)
                return new List<GenericSelectionElement>();
            return list.Where(element => ElementMatches(element, text)).ToList();
        }

        public static bool ElementMatches(GenericSelectionElement genericSelectionElement, string text)
        {
            if (genericSelectionElement == null)
                return false;
            if (string.IsNullOrEmpty(genericSelectionElement.ShowText) && string.IsNullOrEmpty(text))
                return true;
            if (genericSelectionElement.ShowText != null && genericSelectionElement.ShowText.Contains(text))
                return true;
            if (genericSelectionElement.Id != null && genericSelectionElement.Id.ToString().Contains(text))
                return true;
            return false;
        }

        public static List<GenericSelectionElement> GenericSelectionElementList<T>(List<T> list)
            where T : IGenericSelectionElementAsociable
        {
            if (list == null)
                return new List<GenericSelectionElement>();
            return list.Select(variable => new GenericSelectionElement
                {
                    Element = variable.GetElement(), Id = variable.GetId(), ShowText = variable.GetShowText()
                }).ToList();
        }

        public static List<GenericSelectionElement> GenericSelectionElementList(List<string> list)
        {
            if (list == null)
                return new List<GenericSelectionElement>();
            return list.Select(variable => new GenericSelectionElement
            {
                Element = variable,
                Id = variable,
                ShowText = variable
            }).ToList();
        }

        public static List<GenericSelectionElement> GenericSelectionElementList(List<int> list)
        {
            if (list == null)
                return new List<GenericSelectionElement>();
            return list.Select(variable => new GenericSelectionElement
            {
                Element = variable,
                Id = variable,
                ShowText = variable.ToString()
            }).ToList();
        }

        public static List<GenericSelectionElement> GenericSelectionElementList(List<double> list)
        {
            if (list == null)
                return new List<GenericSelectionElement>();
            return list.Select(variable => new GenericSelectionElement
            {
                Element = variable,
                Id = variable,
                ShowText = variable.ToString()
            }).ToList();
        }

        public static List<GenericSelectionElement> GenericSelectionElementList(List<DateTime> list)
        {
            if (list == null)
                return new List<GenericSelectionElement>();
            return list.Select(variable => new GenericSelectionElement
            {
                Element = variable,
                Id = variable,
                ShowText = variable.ToString()
            }).ToList();
        }

        public object GetId()
        {
            return Id;
        }

        public string GetShowText()
        {
            return ShowText;
        }

        public object GetElement()
        {
            return Element;
        }
    }


}
