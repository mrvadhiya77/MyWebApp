var oTable;
$(document).ready(function () {
    oTable = $("#orderTable").DataTable({
        ajax: {
            url: "/Admin/Order/GetData"
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
                        <a href = "/Admin/Order/AddEditOrder?id=${data}"><i class="bi bi-pencil-square"></i></a>`
                    
                }
            }
        ]
    });

});
