window.interop = {
    initComponent: (element, model, dotnetObj) => {
        var compObj = new myComp(model, element);
        compObj.dotnetObj = dotnetObj;
    },

    updateProperties: async (compObj, options) => {
        await compObj.dotnetObj.invokeMethodAsync('UpdateProperties', options);
    },

    setProperties: (element, options) => {
        var compObj = element["myComp"];
        compObj.setProperties(options);
    }
};

(function () {
    "use strict";
    var myComp = (function () {
        function myComp(options, element) {
            this.isChanged = false;
            this.value = null;
            if (element) {
                this.init(options);
                this.wireEvents(element);
                this.render(element);
            }
        };

        myComp.prototype.init = function(options) {
            this.isChanged = options.isChanged;
            this.value = options.value;
        };

        myComp.prototype.render = function(element) {
            element["myComp"] = this;
        };

        myComp.prototype.wireEvents = function(element) {
            element.addEventListener("change", this.onChange);
        };

        myComp.prototype.onChange = function(e) {
            this.myComp.isChanged = true;
            interop.updateProperties(this.myComp, { isChanged: this.myComp.isChanged });
        };

        myComp.prototype.setProperties = function(options) {
            for (var property in options) {
                this[property] = options[property];
            }
        };
        return myComp;
    })();
    window.myComp = myComp;
})();