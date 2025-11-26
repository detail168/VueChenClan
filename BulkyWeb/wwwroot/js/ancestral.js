//var dataTable;

//$(document).ready(function () {
//    loadDataTable();
//});

//            function loadDataTable() {dataTable = $('#tblData').DataTable({
//                "ajax": { url: '/api/admin/ancestral' },
//                "columns": [
//                    { "data": "name", "width": "10%" },
//                    { "data": "side", "width": "8%" },
//                    { "data": "section", "width": "8%" },
//                    { "data": "level", "width": "8%" },
//                    { "data": "position", "width": "8%" },
//                    { "data": "applicant", "width": "10%" },
//                    { "data": "relation", "width": "10%" },
//                    { "data": "mobile_Tel", "width": "18%" },
//                    {
//                        data: 'id',
//                        "render": function (data) {
//                            return `<div class="w-75 btn-group" role="group">
//                     <a href="/admin/ancestral/upsert?id=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Edit</a>
//                     <a onClick=Delete('/api/admin/ancestral/${data}') class="btn btn-danger mx-2"> <i class="bi bi-trash-fill"></i> Delete</a>
//                    </div>`;
//                        },
//                        "width": "20%"
//                    }
//                ]
//            })};

//            function Delete(url) {Swal.fire({
//                title: 'Are you sure? �T�{�R��?',
//                text: "You won't be able to revert this! �L�k�^�_!",
//                icon: 'warning',
//                showCancelButton: true,
//                confirmButtonColor: '#3085d6',
//                cancelButtonColor: '#d33',
//                confirmButtonText: 'Yes, delete it!'
//            }).then((result) => {
//                if (result.isConfirmed) {
//                    $.ajax({
//                        url: url,
//                        type: 'DELETE',
//                        success: function (data) {
//                            dataTable.ajax.reload();
//                            toastr.success(data.message);
//                        }
//                    });
//                }
//            })
//};
