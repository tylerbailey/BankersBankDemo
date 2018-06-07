$(function () {

    //Listener for the Select File button
    //Triggers the hidden file input element
    $("#fileSelectBtn").click(function () {
        $("#fileSelectInput").click();
    });

    //Listener for the hidden file input element
    //When a file is selected the name is copied to
    //the display text box 
    $("#fileSelectInput").on("input", function () {
        var fileName = $("#fileSelectInput").val().split("\\");
        $("#fileNameTextBox").val(fileName[fileName.length-1]);
    });

    //Listener for the file upload button
    //Does some rough validation before attempting the file upload
    $("#fileUploadBtn").click(function () {
        var fileName = $("#fileSelectInput").val();
        if (fileName != "") {
            var extention = fileName.split(".")[1];
            if (extention.toLowerCase() == "csv") {
                if ($("#errorAlert").is(":visible")) {
                    $("#errorAlert").toggleClass("hidden");
                }
                uploadFile();
            }
            else {
                $("#errorAlert").html("Error: file must be a CSV type");
                if (!$("#errorAlert").is(":visible")) {
                    $("#errorAlert").toggleClass("hidden");
                }               
               
            }
        }
        else {
            $("#errorAlert").html("Error: select a file");
            if (!$("#errorAlert").is(":visible")) {
                $("#errorAlert").toggleClass("hidden");
            }
           
        }
    });

    //Calls the external simple upload Jquery plugin to post the 
    //user selected file to the UploadFile method in the home controller
    function uploadFile() {
        $("#fileSelectInput").simpleUpload("/Home/UploadFile", {

            start: function (file) {
                //Resets the progress bar on new upload
                $("#uploadProgressBar").attr('aria-valuenow', 0).css('width',"0%");
            },

            progress: function (progress) {
                //Updates progress bar
                $("#uploadProgressBar").attr('aria-valuenow', Math.round(progress)).css('width', Math.round(progress) + "%");
                $("#uploadProgressBar").html(Math.round(progress) + "%");
            },

            success: function (data) {
                //clears out file name text box and processes returned JSON for table display
                $("#fileNameTextBox").val("");
                $("#uploadProgressBar").html("Upload complete");
                createTable(data);
            },

            error: function (error) {
                //boilerplate returned error logging 
                console.log("upload error: " + error.name + ": " + error.message);
            }
        });

    };

    //Takes the returned JSON string and converts it objects, then builds a 
    //table using data from the objects. If an error occured trying to read
    //the file an error message is displayed instead
    function createTable(data) {
        var responseObject = $.parseJSON(data);
        if(responseObject.Status == true){
        var tableData = '<table class="table table-striped table-bordered"><thead><tr><th>Account Number</th><th>Current Balance</th><th>Last Name/Company Name</th><th>First Name</th><th>Middle Initial</th><th>Account Type</th></tr></thead><tbody>';
        $.each(responseObject.Result, function (index, item) {
            tableData += '<tr><td>' + item.AccountNumber + '</td><td>' + item.CurrentBalance + '</td><td>' + item.LastNameCompanyName + '</td><td>' + item.FirstName + '</td><td>' +
                item.MiddleName + '</td><td>' + item.AccountType + '</td></tr>';
        });
        tableData += '</tbody></table>';
        $("#displayTable").html(tableData);
        }
        else {
            $("#displayTable").html("");
            $("#errorAlert").html("Error: there was a problem reading the file");
            if (!$("#errorAlert").is(":visible")) {
                $("#errorAlert").toggleClass("hidden");
            }
        }
    };
});