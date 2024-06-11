$(document).ready(function () {
    var shortlistedCandidates = [];

    $('.shortlist-checkbox').change(function () {
        var candidateId = $(this).data('id');
        if ($(this).is(':checked')) {
            // Add to shortlist
            shortlistedCandidates.push(candidateId);
            addCandidateToShortlist(candidateId);
        } else {
            // Remove from shortlist
            shortlistedCandidates = shortlistedCandidates.filter(id => id !== candidateId);
            removeCandidateFromShortlist(candidateId);
        }
        updateShortlistCount();
    });

    function addCandidateToShortlist(candidateId) {
        if (!$('#shortlist-' + candidateId).length) {
            var row = $('#jobsTable').find('input[data-id="' + candidateId + '"]').closest('tr');
            var name = row.find('td:nth-child(3)').text();
            var phoneNumber = row.find('td:nth-child(4)').text();
            var email = row.find('td:nth-child(5)').text();

            $('#shortlistTable tbody').append(`
              <tr id="shortlist-${candidateId}">
                <td>${name}</td>
                <td>${phoneNumber}</td>
                <td>${email}</td>
              </tr>
            `);
        }
    }

    function removeCandidateFromShortlist(candidateId) {
        $('#shortlist-' + candidateId).remove();
    }

    function updateShortlistCount() {
        var count = $('#shortlistTable tbody tr').length;
        if (count > 0) {
            $('#scheduleSkillTestBtn').show();
        } else {
            $('#scheduleSkillTestBtn').hide();
        }
    }

    updateShortlistCount();
});

function addShortlist(id) {
    $.ajax({
        url: '/ApplyJob/AddToShortlist',
        type: 'POST',
        data: { id: id },
        success: function (data) {
            addCandidateToShortlist(data.id);
            updateShortlistCount();
        }
    });
}

function removeShortlist(id) {
    $.ajax({
        url: '/ApplyJob/RemoveFromShortlist',
        type: 'POST',
        data: { id: id },
        success: function (data) {
            removeCandidateFromShortlist(data.id);
            updateShortlistCount();
        }
    });
}

function sortTable(columnName, direction = 'asc') {
    var url = window.location.href;
    if (url.includes('?')) {
        url = url.split('?')[0];
    } else {
        url = '/ApplyJob/Index';
    }
    var searchParams = new URLSearchParams(window.location.search);
    searchParams.set('sortBy', columnName);
    searchParams.set('sortOrder', direction);
    window.location.href = url + '?' + searchParams.toString();
}
 
function searchTable() {
    var input, filter, table, tr, td, i, j, txtValue;
    input = document.getElementById("searchInput");
    filter = input.value.toUpperCase();
    table = document.getElementById("jobsTable");
    tr = table.getElementsByTagName("tr");
 
    for (i = 1; i < tr.length; i++) {
        tr[i].style.display = "none";
        td = tr[i].getElementsByTagName("td");
        for (j = 0; j < td.length; j++) {
            if (td[j]) {
                txtValue = td[j].textContent || td[j].innerText;
                if (txtValue.toUpperCase().indexOf(filter) > -1) {
                    tr[i].style.display = "";
                    break;
                }
            }
        }
    }
}

$(document).ready(function () {
    loadFormLinks();
    function loadFormLinks() {
        $.ajax({
            url: '/FormLink/GetFormLinks',
            method: 'GET',
            success: function (data) {
                console.log('Form Links:', data); // Debug line
                var formLinkDropdown = $('#formLink');
                formLinkDropdown.empty();
                formLinkDropdown.append('<option value="">Select Form Link</option>');
                data.forEach(function (link) {
                    formLinkDropdown.append('<option value="' + link.links + '">' + link.title + '</option>');
                });
            },
            error: function (error) {
                console.error('Error loading form links:', error);
            }
        });
    }
});
function showLoader() {
    $('#loader').show();
}

function hideLoader() {
    $('#loader').hide();
}
function scheduleTest() {
    var isValid = true;
    $('#skillTestForm .form-control').each(function () {
        if (!this.checkValidity()) {
            $(this).addClass('is-invalid');
            isValid = false;
        } else {
            $(this).removeClass('is-invalid');
        }
    });
    if (isValid) {
        var testType = $('#testType').val();
        var testDate = $('#testDate').val();
        var startTime = $('#startTime').val();
        var duration = $('#duration').val();
        var formLink = $('#formLink').val();
        var Email = $('#Email').val().split(',').map(email => email.trim());
        var data = {
            testType: testType,
            testDate: testDate,
            startTime: startTime,
            duration: duration,
            selectedLink: formLink,
            email: Email
        };
        dismissModal();
        $('#loader').show();

        $.ajax({
            url: '/TestLink/SaveSelectedLink',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            success: function (response) {
                $('#loader').hide();
                if (response.success) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Success!',
                        text: 'Skill test scheduled successfully.'
                    });
                } else {
                    alert('Failed to schedule skill test.');
                }
            },
            error: function (xhr) {
                $('#loader').hide(); 
                alert('Error scheduling skill test. ' + xhr.responseText);
            }
        });
    }
}


function editItem(itemId) {
    window.location.href ='/FormLink/CreateOrEdit' + '?id=' + itemId;
}


function deleteItem(id) {
    if (confirm('Are you sure you want to delete this item?')) {
        var token = $('input[name="__RequestVerificationToken"]').val();
        $.ajax({
            url: '/FormLink/Delete',
            type: 'POST',
            data: {
                id: id,
                __RequestVerificationToken: token
            },
            success: function (result) {
                if (result.success) {
                    location.reload();
                } else {
                    alert(result.message);
                }
            }
        });
    }
}
function removeShortlist(id) {
    $.ajax({
        url: '/ApplyJob/RemoveFromShortlist',
        type: 'POST',
        data: { id: id },
        success: function (data) {
            $(`#shortlistTable tbody tr[data-id="${data.id}"]`).remove();
            updateShortlistCount();
        }
    });
}

function updateShortlistCount() {
    var count = $('#shortlistTable tbody tr').length;
    if (count > 0) {
        $('#scheduleSkillTestBtn').show();
    } else {
        $('#scheduleSkillTestBtn').hide();
    }
}

$(document).ready(function () {
    updateShortlistCount();
});
function prepareSkillTestModal() {
    var emails = [];
    $('#shortlistTable tbody tr').each(function () {
        var email = $(this).find('td').eq(2).text().trim();
        if (email) {
            emails.push(email);
        }
    });

    $('#Email').val(emails.join(', '));
    $('#skillTestModal').modal('show');
}

$(document).ready(function () {
    $('#skillTestModal').modal({
        backdrop: 'static',
        keyboard: false,
        hide: true
    });
});
function dismissModal() {
    $('#skillTestForm .form-control').removeClass('is-invalid');
    $('#skillTestForm')[0].reset();
    $('#skillTestModal').modal('hide');
    $('body').removeClass('modal-open');
    $('.modal-backdrop').remove();
}
$(document).ready(function () {
    function showNoRecordsModal() {
        $('#noRecordsModal').modal('show');
    }
    if ($('#jobsTable tbody').children().length === 0) {
        showNoRecordsModal();
    }
});
function navigateBack() {
    $('#noRecordsModal').modal('hide');
}    
// Function to load PDF preview
function loadPDF(id) {
    $.ajax({
        url: '/ApplyJob/GetPDFPath',
        type: 'GET',
        data: { id: id },
        success: function (pdfPath) {
            PDFObject.embed(pdfPath, "#pdfPreview", {
                height: "500px"
            });
        }
    });
}
$(document).ready(function () {
    $('#jobsTable').DataTable({
        "paging": true,
        "lengthChange": false,
        "searching": true,
        "ordering": true,
        "info": true,
        "autoWidth": false,
        "pageLength": 10,
        "dom": 'Bfrtip',
        "buttons": [
            {
                extend: 'excelHtml5',
                text: 'Export to Excel',
                className: 'btn btn-success mb-2'
            }
        ]
    });


});