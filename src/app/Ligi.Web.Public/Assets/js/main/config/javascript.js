(function () {
    Array.prototype.insertAt = function (index, item) {
        this.splice(index, 0, item);
        return this;
    };
})();