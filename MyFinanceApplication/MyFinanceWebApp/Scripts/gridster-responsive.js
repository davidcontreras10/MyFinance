var gridster;
var gridsterDragStopEvent = new CustomEvent("gridster-drag-stop-event");

$(function () {
    intializeGridster();
    onWindowResizeFinished();
});


function onWindowResizeFinished() {

    setTimeout(function () {
        updateGridsterSize();
    }, 1000);

}

function updateGridsterSize() {
    var pxGridsterSize = GetGridsterAreaPixels('.gridster');
    var width = GetSuggestedGridsterWidth('.gridster');
    var widthSuggested = GetGridsterWidthSize(width);
    var height = GetGridsterHeightSize(pxGridsterSize.height);
    height = Math.floor(height);
    widthSuggested = Math.floor(widthSuggested);
    gridster.resize_widget_dimensions({
        widget_base_dimensions: [widthSuggested, height]
    });
}

function GetSuggestedGridsterWidth(jquerySelector) {
    var pxGridsterSize = GetGridsterAreaPixels(jquerySelector);
    var widthPx = GetGridsterWidthSize(pxGridsterSize.width);
    var result;
    if (widthPx > pxGridsterSize.width) {
        result = pxGridsterSize.width;
    } else {
        result = widthPx;
    }
    return result;
}

function GetGridsterAreaPixels(jquerySelector) {
    var gridHeight = getGridsterHeightPixels(jquerySelector);
    var gridWidth = getGridsterWidthPixels(jquerySelector);
    return {
        width: gridWidth,
        height: gridHeight
    };
}

function getGridsterHeightPixels(jquerySelector) {
    var bodyHeight = $(window).height();
    var objectOffset = $(jquerySelector).offset();
    var top = objectOffset.top;
    var windowMargin = top + 100;
    //windowMargin = Math.floor(windowMargin);
    var gridHeight;
    if (bodyHeight <= windowMargin) {
        gridHeight = 100;
    } else {
        gridHeight = bodyHeight - windowMargin;
    }
    return gridHeight;
}

function getGridsterWidthPixels(jquerySelector) {
    var bodyWidth = $(jquerySelector).width();
    var objectOffset = $(jquerySelector).offset();
    var left = objectOffset.left;
    var windowMargin = left;
    //windowMargin = Math.floor(windowMargin);
    var gridWidth;
    if (bodyWidth <= windowMargin) {
        gridWidth = 25;
    } else {
        gridWidth = bodyWidth;
    }
    return gridWidth;
}

function GetGridsterWidthSize(pixels) {
    if (pixels) {
        if (pixels <= 100) {
            return 40;
        }
	    var result = (pixels / 2) - (pixels * 0.07);
        //result = Math.floor(result);
        return result;
    }
    return null;
}

function GetGridsterHeightSize(pixels) {
    if (pixels) {
        if (pixels <= 100) {
            return 40;
        }
        var result = (pixels / 4) - 10;
        //result = Math.floor(result);
        return result;
    }
    return null;
}

function GetPixelWidthFromGridster(gridsterDim) {
    if (gridsterDim) {
        var result = (2 * gridsterDim) + 20;
        //result = Math.floor(result);
        return result;
    }
    return null;
}

function debouncer(func, timeout) {
    timeout = timeout || 200;
    var timeoutId;
    return function () {
        var scope = this, args = arguments;
        clearTimeout(timeoutId);
        timeoutId = setTimeout(function () {
            func.apply(scope, Array.prototype.slice.call(args));
        }, timeout);
    };
}

$(window).resize(debouncer(function () {
    onWindowResizeFinished();
}));

function intializeGridster() {
    gridster = $(".gridster ul").gridster({
        widget_base_dimensions: [5, 5],
        widget_margins: [10, 10],
        draggable: {
        	stop: gridsterPositionChanged
        },
        autogrow_cols: true    
    }).data('gridster');
}

function gridsterPositionChanged() {
	document.dispatchEvent(gridsterDragStopEvent);
}
