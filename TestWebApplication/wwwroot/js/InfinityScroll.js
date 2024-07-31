function InfinityScroll(mainElementId, listRowClass, postAction, callbackFilter, postParams) {
    this.mainElementId = mainElementId;
    this.listRowClass = listRowClass;
    this.action = postAction;
    this.params = postParams;
    this.loading = false;
    this.AppendChild = function (firstItem) {
        this.Loading = true;
        this.params.firstItem = firstItem;
        // $("#footer").css("display", "block"); // show loading info
        $.ajax({
            type: 'POST',
            url: self.action,
            data: self.params,
            dataType: "html"

        })
            .done(function (result) {
                if (result) {
                    $("#" + self.mainElementId).append(result);
                    self.loading = false;
                    var rowCount = $('#' + self.mainElementId + ' .' + self.listRowClass).length;
                    if (rowCount) {
                        callbackFilter(rowCount);
                    }
                }
            })

            .fail(function (xhr, ajaxOptions, thrownError) {
                console.log("Error in AddTableLines:", thrownError);
            })

            .always(function () {
                // $("#footer").css("display", "none"); // hide loading info
            });
    }

    var self = this;
    window.onscroll = function (ev) {
        if ((window.innerHeight + window.scrollY) >= document.body.offsetHeight - 1) {
            // User is currently at the bottom of the page
            if (!self.loading) {
                var itemCount = $('#' + self.mainElementId + ' .' + self.listRowClass).length;
                self.AppendChild(itemCount);
            }
        }
    };
    this.AppendChild(0);
}




    
