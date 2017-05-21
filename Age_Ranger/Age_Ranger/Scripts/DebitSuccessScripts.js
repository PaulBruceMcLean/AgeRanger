document.getElementById('AddPerson').addEventListener('click', SubmitNewPerson, true);
document.getElementById('SearchPerson').addEventListener('click', SearchPerson, true);
document.getElementById('DeletePerson').addEventListener('click', DeletePerson, true);
document.getElementById('UpdatePerson').addEventListener('click', UpdatePerson, true);


function UpdatePerson() {

    var firstname = document.getElementById('Update-First-Name').value.toString();
    if (firstname == '' || firstname == null) {
        alert('Provide a First Name');
        return;
    }

    var lastname = document.getElementById('Update-Last-Name').value.toString();
    if (lastname == '' || lastname == null) {
        alert('Provide a Last Name');
        return;
    }

    var age = document.getElementById('Update-Age').value.toString();
    if (age == "" || age == null) {
        alert('Provide a Age');
        return;
    }

    var dbid = document.getElementById('Update-DbID').value.toString();
    if (dbid == "" || dbid == null) {
        alert('Provide a Database ID');
        return;
    }


    var PersonModel = {
        "PersonID": dbid,
        "PersonFirstName": firstname,
        "PersonLastName": lastname,
        "CurrentAge": age
    };
    debugger;
    var xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            alert(this.responseText);
        }
    };
    xhttp.open("POST", "/api/Person/Update", true);
    xhttp.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
    xhttp.send(JSON.stringify(PersonModel));
}

function SubmitNewPerson() {

    var firstname = document.getElementById('First-Name').value.toString();
    if (firstname == '' || firstname == null) {
        alert('Provide a First Name');
        return;
    }

    var lastname = document.getElementById('Last-Name').value.toString();
    if (lastname == '' || lastname == null) {
        alert('Provide a Last Name');
        return;
    }

    var age = document.getElementById('Age').value.toString();
    if (age == "" || age == null) {
        alert('Provide a Age');
        return;
    }


    var PersonModel = {
        "PersonID": 0,
        "PersonFirstName": firstname,
        "PersonLastName": lastname,
        "CurrentAge": age
    };
    debugger;
    var xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            alert(this.responseText);
        }
    };
    xhttp.open("POST", "/Person/Add", true);
    xhttp.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
    xhttp.send(JSON.stringify(PersonModel));
}

function DeletePerson() {
    var DeletePersonID = document.getElementById('Delete-Person').value.toString();
    if (DeletePersonID == '' || DeletePersonID == null) {
        alert('Provide a Database Record ID');
        return;
    }
    var xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            alert(this.responseText);
        }
    };
    xhttp.open("POST", "/Person/Delete/" + DeletePersonID, true);
    xhttp.send();
}

function SearchPerson() {
    var firstnamesearch = document.getElementById('First-Name-Search').value.toString();
    if (firstnamesearch == '' || firstnamesearch == null) {
        alert('Please Supply First Name')
        return;
    }
    var xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            $('#People-Table > tbody > tr > td').parent('tr').empty();
            ProcessSearchresults(this.responseText);
        }
    };
    xhttp.open("GET", "/Person/Find/" + firstnamesearch, true);
    xhttp.send();
}

function ProcessSearchresults(data) {
    var Resultsobj = JSON.parse(data);
    var table = document.getElementById("People-Table");
    for (var prop in Resultsobj) {
        var person = Resultsobj[prop];
        person.forEach(function (entry) {
            $('#People-Table tr:last').after('<tr><td>' + entry['PersonFirstName'].toString() + '</td><td>' + entry['PersonLastName'].toString() + '</td><td>' + entry['CurrentAge'].toString() + '</td><td>' + entry['AgeDescription'].toString() + '</td><td>' + entry['PersonID'].toString() + '</td></tr>');
        });
    }
}
