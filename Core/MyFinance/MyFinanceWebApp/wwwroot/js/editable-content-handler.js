$(function () {
    
    $.EditableContentHandlerClass = function(jquerySelector, onChange) {
        this.jquerySelector = jquerySelector;
        this.onChange = onChange;
        this.loadEditableContent = loadEditableContent;
        this.startContent = startContent;

    }

    $.EditableContentHandlerClass.prototype = {
        InitEvents: function() {
            //`this` references the instance object inside of an instace's method,
            //however `this` is set to reference a DOM element inside jQuery event
            //handler functions' scope. So we take advantage of JS's lexical scope
            //and assign the `this` reference to another variable that we can access
            //inside the jQuery handlers
            this.loadEditableContent(this);
        }
    };

    var loadEditableContent = function(object) {
        object.startContent(object);
        $(object.jquerySelector).blur(function (data) {
            var contentChanged = isContentChanged(object, data.target);
            if (contentChanged) {
                contentChanged.content = $(data.target).html();
                object.onChange(contentChanged.element);
            }
        });
    }

    var isContentChanged = function(object, element) {
        var contentElement = getContentElement(object, element);
        if (contentElement.content != $(element).html()) {
            return contentElement;
        }

        return false;
    }

    var getContentElement = function(object, element) {
        var contentArray = object.contentArray;
        for (var i = 0; i < contentArray.length; i++) {
            var objectElement = contentArray[i];
            if (element === objectElement.element) {
                return objectElement;
            } 
        }

        return null;
    }

    var startContent = function (object) {
        object.contentArray = new Array();
        var contentArray = object.contentArray;
        var jquerySelector = object.jquerySelector;
        var elements = $(jquerySelector);
        for (var i = 0; i < elements.length; i++) {
            var element = elements[i];
            var content = $(element).html();
            var contentElement = {
                element: element,
                content: content
            };

            contentArray.push(contentElement);
        }
    }
});