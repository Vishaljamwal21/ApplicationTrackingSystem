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

    // Initial check to show/hide the Schedule Skill Test button
    updateShortlistCount();
});

// Separate functions for adding and removing shortlist candidates
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



function scheduleTest() {
    var testType = $('#testType').val();
    var testDate = $('#testDate').val();
    var startTime = $('#startTime').val();
    var duration = $('#duration').val();
    var formLink = $('#formLink').val(); // Get the selected form link from the dropdown

    var data = {
        testType: testType,
        testDate: testDate,
        startTime: startTime,
        duration: duration,
        selectedLink: formLink // Include the selected form link in the data
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