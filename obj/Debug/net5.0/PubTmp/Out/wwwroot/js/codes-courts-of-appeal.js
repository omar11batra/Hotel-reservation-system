/**
 * Page court of appeal
 */

'use strict';

// Datatable (jquery)
$(function () {
    // Variable declaration for table
    var dt_codes_table = $('.datatables-Court-Of-Appeal'), //name of the table
        select2 = $('.select2') //for any filter or drop-down-list


    // primary courts datatable
    if (dt_codes_table.length) {
        var dt_codes = dt_codes_table.DataTable({
            ajax: assetsPath + 'json/codes-courts-appeal.json', // JSON file to add data
            columns: [
                // columns according to JSON
                {
                    data: ''
                },
                {
                    data: 'id'
                },
                {
                    data: 'name'
                },
                
                {
                    data: 'actions'
                }
            ],
            columnDefs: [{
                    // For Responsive
                    className: 'control',
                    searchable: false,
                    orderable: true,
                    responsivePriority: 2,
                    targets: 0,
                    render: function (data, type, full, meta) {
                        return '';
                    }
                },
                {
                    // id
                    targets: 1,

                    render: function (data, type, full, meta) {
                        var $id = full['id'];

                        return '<span class="fw-light">' + $id + '</span>';
                    }
                },
                {
                    // name
                    targets: 2,
                    render: function (data, type, full, meta) {
                        var $name = full['name'];

                        return '<span class="fw-light">' + $name + '</span>';
                    }
                },
                

                {
                    // Actions
                    targets: -1,
                    title: 'الاجراءات ',
                    searchable: false,
                    orderable: true,
                    render: function (data, type, full, meta) {
                        return (
                            '<div class="d-inline-block align-end">' +
                            '<button class="btn btn-sm btn-icon dropdown-toggle hide-arrow" data-bs-toggle="dropdown"><i class="bx bx-dots-vertical-rounded"></i></button>' +
                            '<div class="dropdown-menu dropdown-menu-start">' +
                            '<a href="javascript:;" data-bs-toggle="offcanvas" data-bs-target="#offcanvasEditCourtOfAppeal"' +
                            '" class="dropdown-item"><i class="fa fa-sm fa-pen me-2"></i>تعديل</a>' +
                            '<div class="dropdown-divider"></div>' +
                            '<a href="javascript:;" class="dropdown-item text-danger delete-record"><i class="fa fa-sm fa-trash me-2"></i>حذف</a></div>' +
                            '</div>' +
                            '</div>'
                        );
                    }
                }
            ],
            order: [
                [1, 'desc']
            ],
            dom: '<"row mx-3"' +
                '<"col-md-2"<"me-3"l>>' +
                '<"col-md-6"<"dt-action-buttons text-xl-end text-lg-start text-md-end text-start d-flex align-items-center justify-content-start flex-md-row flex-column mb-3 mb-md-0"f>>' +
                '<"col-md-4"<"dt-action-buttons text-xl-end text-lg-start text-md-end text-start d-flex align-items-center justify-content-end flex-md-row flex-column mb-3 mb-md-0"B>>' +
                '>t' +
                '<"row mx-2"' +
                '<"col-sm-12 col-md-6"i>' +
                '<"col-sm-12 col-md-6"p>' +
                '>',
            language: {
                sLengthMenu: 'عرض  _MENU_  المدخلات',
                search: '',
                searchPlaceholder: 'البحث..',
                info: ''
            },
            // Buttons with Dropdown
            buttons: [

                {
                    text: '<i class="fa fa-plus me-0 me-lg-2"></i><span class="d-none d-lg-inline-block">إضافة عنصر جديد</span>',
                    className: 'add-new btn btn-primary m-xl-3 justify-content-end',
                    attr: {
                        'data-bs-toggle': 'offcanvas',
                        'data-bs-target': '#offcanvasAddCourtOfAppeal'
                    }
                }
            ],
            // For responsive popup
            responsive: {
                details: {
                    display: $.fn.dataTable.Responsive.display.modal({
                        header: function (row) {
                            var data = row.data();
                            return 'Details of ' + data['name'];
                        }
                    }),
                    type: 'column',
                    renderer: function (api, rowIdx, columns) {
                        var data = $.map(columns, function (col, i) {
                            return col.title !== '' // ? Do not show row in modal popup if title is blank (for check box)
                                ?
                                '<tr data-dt-row="' +
                                col.rowIndex +
                                '" data-dt-column="' +
                                col.columnIndex +
                                '">' +
                                '<td>' +
                                col.title +
                                ':' +
                                '</td> ' +
                                '<td>' +
                                col.data +
                                '</td>' +
                                '</tr>' :
                                '';
                        }).join('');

                        return data ? $('<table class="table"/><tbody />').append(data) : false;
                    }
                }
            }
        });
    }

    // Delete Record
    $('.datatables-Court-Of-Appeal tbody').on('click', '.delete-record', function () {
        dt_codes.row($(this).parents('tr')).remove().draw();
    });

    // Filter form control to default size
    // ? setTimeout used for multilingual table initialization
    setTimeout(() => {
        $('.dataTables_filter .form-control').removeClass('form-control-sm');
        $('.dataTables_length .form-select').removeClass('form-select-sm');
    }, 300);
});

// Validation 
(function () {
    var AddCourtForm = document.getElementById('AddCourtOfAppealForm');


    // Add New User Form Validation
    const fv = FormValidation.formValidation(AddCourtForm, {
        fields: {
            CourtNum: {
                validators: {
                    notEmpty: {
                        message: 'الرجاء إدخال الرقم '
                    }
                }
            },
            Courtname: {
                validators: {
                    notEmpty: {
                        message: 'الرجاء إدخال اسم محكمة الإستئناف'
                    }
                }
            }
        },
        plugins: {
            trigger: new FormValidation.plugins.Trigger(),
            bootstrap5: new FormValidation.plugins.Bootstrap5({
                // Use this for enabling/changing valid/invalid class
                eleValidClass: '',
                rowSelector: function (field, ele) {
                    // field is the field name & ele is the field element
                    return '.mb-3';
                }
            }),
            submitButton: new FormValidation.plugins.SubmitButton(),
            // Submit the form when all fields are valid
            // defaultSubmit: new FormValidation.plugins.DefaultSubmit(),
            autoFocus: new FormValidation.plugins.AutoFocus()
        }
    });
})();