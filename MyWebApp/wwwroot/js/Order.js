var oTable;
$(document).ready(function () {

    // Search Window Location
    var url = window.location.search;
    if (url.includes("pending")) {
        OrderTable("pending");
    }
    else if (url.includes("approved")) {
        OrderTable("approved")
    }
    else if (url.includes("underprocess")) {
        OrderTable("underprocess")
    }
    else if (url.includes("shipped")) {
        OrderTable("shipped")
    }
    else {
        OrderTable("all")
    }
});
function OrderTable(status) {
    oTable = $("#orderTable").DataTable({
        ajax: {
            url: "/Admin/Order/GetData?status=" + status
        },
        columns: [
            { data: 'name' },
            { data: 'phone' },
            { data: 'orderStatus' },
            { data: 'orderTotal' },
            {
                data: "id",
                render: function (data) {
                    return `
                        <a href = "/Admin/Order/OrderDetail?id=${data}"><i class="bi bi-pencil-square"></i></a>`

                }
            }
        ]
    });
}