function showFormLink() {
    var formLink = document.getElementById('formLink').value;
    var formLinkSection = document.getElementById('formLinkSection');
    var embeddedForm = document.getElementById('embeddedForm');

    if (formLink === 'level1') {
        embeddedForm.src = "https://forms.office.com/Pages/ResponsePage.aspx?id=DQSIkWdsW0yxEjajBLZtrQAAAAAAAAAAAANAAU9ZmUlUREtVWjZHRUFTQThIRTUyNzNGUzUxNEdFNS4u&embed=true";
        formLinkSection.style.display = 'block';
    } else if (formLink === 'level2') {
        embeddedForm.src = "https://forms.office.com/Pages/ResponsePage.aspx?id=exampleLevel2Link&embed=true"; // Replace with actual URL
        formLinkSection.style.display = 'block';
    } else {
        formLinkSection.style.display = 'none';
    }
}

function scheduleTest() {
    var testType = $('#testType').val();
    var testDate = $('#testDate').val();
    var startTime = $('#startTime').val();
    var duration = $('#duration').val();
    var formLink = $('#formLink').val();

    var selectedLink = '';
    if (formLink === 'level1') {
        selectedLink = "https://forms.office.com/Pages/ResponsePage.aspx?id=DQSIkWdsW0yxEjajBLZtrQAAAAAAAAAAAANAAU9ZmUlUREtVWjZHRUFTQThIRTUyNzNGUzUxNEdFNS4u";
    } else if (formLink === 'level2') {
        selectedLink = "https://forms.office.com/Pages/ResponsePage.aspx?id=exampleLevel2Link"; // Replace with actual URL
    }

    var data = {
        testType: testType,
        testDate: testDate,
        startTime: startTime,
        duration: duration,
        selectedLink: selectedLink
    };

    $.ajax({
        url: '/TestLink/SaveSelectedLink',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (response) {
            if (response.success) {
                alert('Skill test scheduled successfully.');
                $('#skillTestModal').modal('hide');
            } else {
                alert('Failed to schedule skill test.');
            }
        },
        error: function () {
            alert('Error scheduling skill test.');
        }
    });
}
// Function to sort table by column name and direction
function sortTable(columnName, direction = 'asc') {
    var url = window.location.href;
    if (url.includes('?')) {
        url = url.split('?')[0];
    } else {
        url = '@Url.Action("Index", "ApplyJob")';
    }
    var searchParams = new URLSearchParams(window.location.search);

    // Set sorting parameters in the URL
    searchParams.set('sortBy', columnName);
    searchParams.set('sortOrder', direction);
    window.location.href = url + '?' + searchParams.toString();
}
// Function to filter table based on search input
$('#searchInput').keyup(function () {
    var searchText = $(this).val().toLowerCase();
    $('#jobsTable tbody tr').each(function () {
        var found = false;
        $(this).find('td').each(function () {
            if ($(this).text().toLowerCase().indexOf(searchText) >= 0) {
                found = true;
                return false;
            }
        });
        $(this).toggle(found); // Show/hide row based on search result
    });
});

function loadPDF(id) {
    $.ajax({
        url: '/ApplyJob/GetPDFPath', // Update URL to match the correct endpoint
        type: 'GET',
        data: { id: id },
        success: function (response) {
            // Check if response is successful and contains PDF path
            if (response) {
                // Embed PDF using PDFObject
                PDFObject.embed(response, "#pdfPreview", {
                    height: "500px"
                });
            } else {
                console.error('PDF path not found.');
            }
        },
        error: function (xhr, status, error) {
            console.error('Error loading PDF:', error);
        }
    });
}


// Handle shortlist checkbox click
$('.shortlist-checkbox').change(function () {
    var id = $(this).data('id');
    if ($(this).is(':checked')) {
        addShortlist(id);
    } else {
        removeShortlist(id);
    }
});

function addShortlist(id) {
    $.ajax({
        url: '/ApplyJob/AddToShortlist',
        type: 'POST',
        data: { id: id },
        success: function (data) {
            var row = `<tr data-id="${data.id}"><td>${data.name}</td><td>${data.phoneNumber}</td><td>${data.email}</td></tr>`;
            $('#shortlistTable tbody').append(row);
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

// Initial check to show/hide the Schedule Skill Test button
$(document).ready(function () {
    updateShortlistCount();
});