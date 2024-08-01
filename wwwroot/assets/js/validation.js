/**
 * Page User List
 */

'use strict';

// Datatable (jquery)
$(function () {
  // Variable declaration for table
  var dt_user_table = $('.datatables-users'),
    select2 = $('.select2'),
    userView = 'app-user-view-account.html',
    statusObj = {
      1: { title: 'Pending', class: 'bg-label-warning' },
      2: { title: 'Active', class: 'bg-label-success' },
      3: { title: 'Inactive', class: 'bg-label-secondary' }
    };

  if (select2.length) {
    var $this = select2;
    $this.wrap('<div class="position-relative"></div>').select2({
      placeholder: 'Select Country',
      dropdownParent: $this.parent()
    });
  }

  // Users datatable
  if (dt_user_table.length) {
    var dt_user = dt_user_table.DataTable({
      ajax: assetsPath + 'json/user.json', // JSON file to add data
      columns: [
        // columns according to JSON
        { data: '' },
        { data: 'id' },
        { data: 'name' },
        { data: 'status' },
        { data: 'action' }
      ],
      columnDefs: [
        {
          // For Responsive
          className: 'control',
          searchable: false,
          orderable: false,
          responsivePriority: 2,
          targets: 0,
          render: function (data, type, full, meta) {
            return '';
          }
        },
       
        
        {
          // Plans
          targets: 1,
          render: function (data, type, full, meta) {
            var $id = full['id'];

            return '<span class="fw-semibold">' + $id + '</span>';
          }
        },
        {
            // Plans
            targets: 2,
            render: function (data, type, full, meta) {
              var $name = full['name'];
  
              return '<span>' + $name + '</span>';
            }
          },
          {
            // User Status
            targets: 3,
          render: function (data, type, full, meta) {
            var $status = full['status'];
            var statusBadgeObj = {
              1:
                '<span class="badge bg-label-success">نشط</span>',
              2:
              '<span class="badge bg-label-danger">غير نشط</span>',
             };
            return "<span class='text-truncate d-flex align-items-center'>" + statusBadgeObj[$status] +'</span>';
          }
          },
       
        {
          // Actions
          targets: -1,
          title: 'الإجراءات',
          searchable: false,
          orderable: false,
          render: function (data, type, full, meta) {
           
            return (
              '<div class="d-inline-block align-end">' +
              '<button class="btn btn-sm btn-icon dropdown-toggle hide-arrow" data-bs-toggle="dropdown"><i class="bx bx-dots-vertical-rounded"></i></button>' +
              '<div class="dropdown-menu dropdown-menu-start">' +
              '<a href="javascript:;" data-bs-toggle="offcanvas" data-bs-target="#offcanvasEditUser"' +
              '" class="dropdown-item"><i class="fa fa-sm fa-pen me-2"></i>تعديل</a>' +
             
              '<a href="javascript:;" data-bs-toggle="modal" data-bs-target="#userDetailModal"' +
              '" class="dropdown-item"><i class="fa fa-sm fa-info-circle me-2"></i>عرض التفاصيل</a>' +
              '<div class="dropdown-divider"></div>' +
              '<a href="javascript:;" class="dropdown-item text-danger delete-record"><i class="fa fa-sm fa-trash me-2"></i>حذف</a></div>' +
              '</div>' +
              '</div>'
          );
          }
        }
      ],
      order: [[1, 'desc']],
      dom:
      '<"row mx-3"' +
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
          text:
						'<i class="fa fa-plus me-0 me-lg-2"></i><span class="d-none d-lg-inline-block">إضافة عنصر جديد</span>',
					className: 'add-new btn btn-primary m-xl-3 justify-content-end',
          attr: {
            'data-bs-toggle': 'offcanvas',
            'data-bs-target': '#offcanvasAddUser'
          }
        }
      ],
      // For responsive popup
      responsive: {
        details: {
          display: $.fn.dataTable.Responsive.display.modal({
            header: function (row) {
              var data = row.data();
              return 'Details of ' + data['full_name'];
            }
          }),
          type: 'column',
          renderer: function (api, rowIdx, columns) {
            var data = $.map(columns, function (col, i) {
              return col.title !== '' // ? Do not show row in modal popup if title is blank (for check box)
                ? '<tr data-dt-row="' +
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
                    '</tr>'
                : '';
            }).join('');

            return data ? $('<table class="table"/><tbody />').append(data) : false;
          }
        }
      },
      initComplete: function () {
        // Adding role filter once table initialized
        this.api()
          .columns(2)
          .every(function () {
            var column = this;
            var select = $(
              '<select id="UserRole" class="form-select text-capitalize"><option value=""> Select Role </option></select>'
            )
              .appendTo('.user_role')
              .on('change', function () {
                var val = $.fn.dataTable.util.escapeRegex($(this).val());
                column.search(val ? '^' + val + '$' : '', true, false).draw();
              });

            column
              .data()
              .unique()
              .sort()
              .each(function (d, j) {
                select.append('<option value="' + d + '">' + d + '</option>');
              });
          });
        // Adding plan filter once table initialized
        this.api()
          .columns(3)
          .every(function () {
            var column = this;
            var select = $(
              '<select id="UserPlan" class="form-select text-capitalize"><option value=""> Select Plan </option></select>'
            )
              .appendTo('.user_plan')
              .on('change', function () {
                var val = $.fn.dataTable.util.escapeRegex($(this).val());
                column.search(val ? '^' + val + '$' : '', true, false).draw();
              });

            column
              .data()
              .unique()
              .sort()
              .each(function (d, j) {
                select.append('<option value="' + d + '">' + d + '</option>');
              });
          });
        // Adding status filter once table initialized
        this.api()
          .columns(5)
          .every(function () {
            var column = this;
            var select = $(
              '<select id="FilterTransaction" class="form-select text-capitalize"><option value=""> Select Status </option></select>'
            )
              .appendTo('.user_status')
              .on('change', function () {
                var val = $.fn.dataTable.util.escapeRegex($(this).val());
                column.search(val ? '^' + val + '$' : '', true, false).draw();
              });

            column
              .data()
              .unique()
              .sort()
              .each(function (d, j) {
                select.append(
                  '<option value="' +
                    statusObj[d].title +
                    '" class="text-capitalize">' +
                    statusObj[d].title +
                    '</option>'
                );
              });
          });
      }
    });
  }

  // Delete Record
  $('.datatables-users tbody').on('click', '.delete-record', function () {
    dt_user.row($(this).parents('tr')).remove().draw();
  });

  // Filter form control to default size
  // ? setTimeout used for multilingual table initialization
  setTimeout(() => {
    $('.dataTables_filter .form-control').removeClass('form-control-sm');
    $('.dataTables_length .form-select').removeClass('form-select-sm');
  }, 300);
});

// Validation & Phone mask
(function () {
  const phoneMaskList = document.querySelectorAll('.phone-mask'),
    addNewUserForm = document.getElementById('addNewUserForm');

  // Phone Number
  if (phoneMaskList) {
    phoneMaskList.forEach(function (phoneMask) {
      new Cleave(phoneMask, {
        phone: true,
        phoneRegionCode: 'US'
      });
    });
  }
  // Add New User Form Validation
  const fv = FormValidation.formValidation(multiStepsForm, {
    fields: {
      multiStepsFirstName: {
        validators: {
          notEmpty: {
            message: 'الرجاء إدخال اسم المستخدم '
          }
        }
      },
      userPassword: {
        validators: {
          notEmpty: {
            message: 'الرجاء إدخال كلمة المرور'
          }
        }
      },
      userConfirmPassword: {
        validators: {
          notEmpty: {
            message: 'الرجاء تأكيد كلمة المرور'
                     },
          identical: {
            compare: function () {
              return addNewUserForm.querySelector('[name="userPassword"]').value;
            },
            message: 'كلمة المرور غير متطابقة مع التأكيد '
          }
        }
      },
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
  }
  )
  ;
})();
