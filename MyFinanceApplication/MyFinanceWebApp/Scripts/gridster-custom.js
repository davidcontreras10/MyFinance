(function ($) {

    Gridster.prototype.resize_widget_dimensions = function (options) {
        if (options.widget_margins) {
            this.options.widget_margins = options.widget_margins;
        }

        if (options.widget_base_dimensions) {
            this.options.widget_base_dimensions = options.widget_base_dimensions;
        }

        this.min_widget_width = (this.options.widget_margins[0] * 2)
            + this.options.widget_base_dimensions[0];
        this.min_widget_height = (this.options.widget_margins[1] * 2)
            + this.options.widget_base_dimensions[1];

        this.$widgets.each($.proxy(function (i, widget) {
            var $widget = $(widget);
            var data = $widget.data();
            this.resize_widget($widget, data.sizex, data.sizey);
        }, this));

        this.generate_grid_and_stylesheet();
        
        this.set_dom_grid_height();
        this.get_widgets_from_DOM();
        return false;
    };
    
})(jQuery);