const uploadBtn = document.getElementById('upload-btn');
const fileInput = document.getElementById('csv-file');
const tableContainer = document.getElementById('table-container');
document.addEventListener('DOMContentLoaded', () => {
    fetch('https://localhost:44373/api/csvdata')
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            const table = createTable(data);
            tableContainer.innerHTML = '';
            tableContainer.appendChild(table);
        })
        .catch(error => {
            console.error('Fetch error:', error);
        });
});
uploadBtn.addEventListener('click', () => {
    const file = fileInput.files[0];
    if (!file) {
        alert('Please select a file.');
        return;
    }
    const formData = new FormData();
    formData.append('file', file);
    fetch('https://localhost:44373/api/csvdata', {
        method: 'POST',
        body: formData
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            const table = createTable(data);
            tableContainer.innerHTML = '';
            tableContainer.appendChild(table);
        })
        .catch(error => {
            console.error('Fetch error:', error);
        });
});
function createTable(data) {
    const table = document.createElement('table');
    table.setAttribute("id", "table-id");
    const thead = table.createTHead();

    const headerRow = thead.insertRow();
    const columnNames = ['Id', 'Name', 'Date of birth', 'Married', 'Phone', 'Salary'] ;

    columnNames.forEach(key => {
        const headerCell = document.createElement('th');
        headerCell.textContent = key;
        headerRow.appendChild(headerCell);
    });

    const tbody = document.createElement('tbody');
    table.appendChild(tbody);

    data.forEach(rowData => {
        const row = tbody.insertRow();
        Object.values(rowData).forEach(value => {
            const cell = row.insertCell();
            cell.textContent = value;
            cell.setAttribute('contenteditable', true);
            cell.setAttribute('data-original-value', value);
        });
        const saveBtn = document.createElement('button');
        saveBtn.textContent = 'Save changes';
        saveBtn.addEventListener('click', () => {
            saveRow(row);
        });
        row.appendChild(saveBtn);

        const deleteBtn = document.createElement('button');
        deleteBtn.textContent = 'Delete';
        deleteBtn.addEventListener('click', () => {
            deleteRow(row);
        });
        row.appendChild(deleteBtn);
    });

    addTableHeaderClickHandlers(table);

    return table;
}
var filterName = document.getElementById("filterName");
var filterDate = document.getElementById("filterDate");
var filterPhone = document.getElementById("filterPhone");
var filterSalary = document.getElementById("filterSalary");
var filterMarried = document.getElementById("filterMarried");
var filterNotMarried = document.getElementById("filterNotMarried");

function filterTable() {
    var table = document.getElementById("table-id");
    var nameValue = filterName.value.toUpperCase();
    var dateValue = filterDate.value.toUpperCase();
    var marriedValue = filterMarried.checked;
    var notmarriedValue = filterNotMarried.checked;
    var phoneValue = filterPhone.value.toUpperCase();
    var salaryValue = filterSalary.value;
    var rows = table.getElementsByTagName("tr");

    for (var i = 1; i < rows.length; i++) {
        var cells = rows[i].getElementsByTagName("td");
        var nameMatch = cells[1].innerHTML.toUpperCase().indexOf(nameValue) > -1;
        var dateMatch = cells[2].innerHTML.toUpperCase().indexOf(dateValue) > -1;
        var marriedMatch = cells[3].innerHTML.toUpperCase() === (marriedValue ? "TRUE" : "FALSE");
        var notmarriedMatch = cells[3].innerHTML.toUpperCase() === (notmarriedValue ? "FALSE" : "TRUE");
        var phoneMatch = cells[4].innerHTML.toUpperCase().indexOf(phoneValue) > -1;
        var salaryMatch = parseFloat(cells[5].innerHTML) >= parseFloat(salaryValue);
        
        if (nameMatch && dateMatch && phoneMatch && (isNaN(parseFloat(salaryValue))?true:salaryMatch) && (marriedMatch || notmarriedMatch)) {
            rows[i].style.display = "";
        } else {
            rows[i].style.display = "none";
        }
    }
}
function sortTable(table, columnIndex, ascending) {
    const tbody = table.querySelector('tbody');
    const rows = Array.from(tbody.querySelectorAll('tr'));

    rows.sort((a, b) => {
        const aValue = a.cells[columnIndex].textContent.trim();
        const bValue = b.cells[columnIndex].textContent.trim();
        if (aValue < bValue) return ascending ? -1 : 1;
        if (aValue > bValue) return ascending ? 1 : -1;
        return 0;
    });

    rows.forEach(row => tbody.appendChild(row));
}

function addTableHeaderClickHandlers(table) {
    const headers = table.querySelectorAll('th');
    headers.forEach((header, index) => {
        header.addEventListener('click', () => {
            const ascending = header.getAttribute('data-order') === 'asc';
            sortTable(table, index, ascending);
            header.setAttribute('data-order', ascending ? 'desc' : 'asc');
        });
    });
}

function deleteRow(id) {
    fetch(`https://localhost:44373/api/csvdata/${id}`, {
        method: 'DELETE'
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            console.log('Row deleted successfully');
        })
        .catch(error => {
            console.error('Fetch error:', error);
        });
}
function saveRow(row) {
    var cells = row.getElementsByTagName("td");
    const id = parseInt(cells[0].innerHTML)
    const name = cells[1].innerHTML;
    const date = Date.parse(cells[2].innerHTML);
    const married = Boolean(cells[3].innerHTML);
    const phone = cells[4].innerHTML;
    const salary = parseFloat(cells[5].innerHTML);
    const data = {
        name: name,
        date: date,
        married: married,
        phone: phone,
        salary: salary
    };
    fetch(`https://localhost:44373/api/csvdata/${id}`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        return response.json();
    })
    .then(data => {
        const updatedRow = createRow(data);
        row.replaceWith(updatedRow);
    })
    .catch(error => {
        console.error('Fetch error:', error);
    });
}
