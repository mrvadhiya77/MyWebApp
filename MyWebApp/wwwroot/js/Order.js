var oTable;
$(document).ready(function () {

    // Search Window Location
    var url = window.location.search;
    if (url.includes("pending")) {
        OrderTable("pending");
    } else {
        if (url.includes("approved")) {
            OrderTable("approved")
        } else {
            OrderTable();
        }
    }
   
});
function OrderTable(status) {
    let ordUrl = status == undefined || status == "" || status ==null ? "/Admin/Order/GetData" : "/Admin/Order/GetData?status=" + status;
    oTable = $("#orderTable").DataTable({
        ajax: {
            url: ordUrl
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