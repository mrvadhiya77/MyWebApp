var pTable;
$(document).ready(function () {
    pTable = $("#productTable").DataTable({
            ajax: {
                url : "/Admin/Product/GetData"
            },
            columns: [
                { data: 'name' },
                { data: 'description' },
                { data: 'price' },
                { data: 'category.name' }
            ]
    });
});