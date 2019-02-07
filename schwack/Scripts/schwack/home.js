var glb_User = '';
var glb_MessageTo = '';

$().ready(function () {
    setupEventHandlers();
    setInterval(getUsers, 1000);
    setInterval(getMessages, 1000);
});

$(window).on('unload', function () {
    signOut();
});

function setupEventHandlers() {
    $('#txtWho').unbind('keypress');
    $('#txtWho').keypress(function (e) {
        if (e.which == 13) {
            var data = $('#txtWho')[0].value;
            signIn(data);
        }
    });

    $('#btnSignIn').unbind('click');
    $('#btnSignIn').click(function () {
        var data = $('#txtWho')[0].value;
        signIn(data);
    })

    $('#txtMessage').unbind('keypress');
    $('#txtMessage').keypress(function (e) {
        if (e.which == 13) {
            postMessage();
        }
    });

    $('#btnSend').unbind('click');
    $('#btnSend').click(function () {
        postMessage();
    });

    $('#lbOnline').on('change', function () {
        glb_MessageTo = $('#lbOnline option:selected')[0].id;
    });
}



function signIn(who) {
    clearMessages();

    $.ajax({
        type: 'POST',
        url: urlSignIn,
        async: false,
        data: { who: who },
        success: success_SignIn,
        error: error_ajax
    });
}
function success_SignIn(data) {
    if (data.success) {
        glb_User = data.user.Id;
        $('#btnSignIn').hide();
        $('#btnSend').show();
        $('#txtWho').prop('disabled', 'disabled');

        populateUsers(data.onlineUsers);
    }
    else {
        var html = messageBox('Warning', 'Error', data.message);
        $('#messages').html(html);
    }
}

function signOut() {
    $.ajax({
        type: 'POST',
        url: urlSignOut,
        async: false,
        data: { id: glb_User },
        success: success_SignIn,
        error: error_ajax
    });
}

function getUsers() {
    $.ajax({
        type: 'POST',
        url: urlGetUsers,
        async: true,
        success: success_GetUsers,
        error: error_ajax
    });
}
function success_GetUsers(data) {
    if (data.success) {
        populateUsers(data.users);
    }
    else {
        var html = messageBox('Warning', 'Error', 'Error occurred getting users.');
        $('#messages').html(html);
    }
}

function postMessage() {
    if (glb_MessageTo.length == 0) {
        var html = messageBox('Error', 'Who ya sending this to, bud?')
        $('#messages').html(html);
        return;
    }

    var from = glb_User;
    var to = glb_MessageTo;
    var message = $('#txtMessage').val();

    var data = {
        from: from,
        to: to,
        message: message
    };

    $.ajax({
        type: 'POST',
        url: urlPostMessage,
        async: false,
        data: data,
        success: success_postMessage,
        error: error_ajax
    });
}
function success_postMessage(data) {
    if (data.success) {
        $('#txtMessage').prop('value', '');

        var message = '<div class="row">';
        message += '<div class="col-sm-2"><strong><i>' + data.message.PostTime.toDotNetDateTime() + '</strong></i></div>';
        message += '<div class="col-sm-2"><strong><i>' + data.message.From.Name + '</strong></i></div>';
        message += '<div class="col-sm-8">' + data.message.MessageText + '</div></div>';

        $('#lblConversation').prepend(message + '\r\n');
    }
    else {
        var html = messageBox('Warning', 'Error', 'Error occurred posting message');
        $('#messages').html(html);
    }
}

function getMessages() {
    $.ajax({
        type: 'POST',
        url: urlGetMessages,
        async: false,
        data: { forId: glb_User },
        success: success_getMessages,
        error: error_ajax
    });
}

function success_getMessages(data) {
    if (data.success) {
        if (data.messages.length > 0) {
            $.each(data.messages, function () {
                var message = '<div class="row">';
                message += '<div class="col-sm-2"><strong><i>' + this.PostTime.toDotNetDateTime() + '</strong></i></div>';
                message += '<div class="col-sm-2"><strong><i>' + this.From.Name + '</strong></i></div>';
                message += '<div class="col-sm-8">' + this.MessageText + '</div></div>';

                $('#lblConversation').prepend(message + '\r\n');
            });
        }
    } else {
        var html = messageBox('Warning', 'Error', 'Error occurred getting messages');
        $('#messages').html(html);
    }
}

function populateUsers(users) {
    $('#lbOnline').empty();
    $.each(users, function () {
        $('#lbOnline').append('<option id="' + this.Id + '">' + this.Name + '</option>')
    });

    if (glb_MessageTo.length > 0) {
        $('#lbOnline option[id=' + glb_MessageTo + ']').prop('selected', true);
    }
}



String.prototype.toDotNetDateTime = function () {
    if (this == '') {
        return '';
    }

    var d = new Date(parseInt(this.substr(6)));
    var dateString = d.getMonth() + 1 + '/' + d.getDate() + '/' + d.getFullYear() + ' ' + d.getHours().toString().padLeft('0', 2) + ':' + d.getMinutes().toString().padLeft('0', 2)

    return dateString;
}

String.prototype.padLeft = function (padString, length) {
    var str = this;
    while (str.length < length)
        str = padString + str;
    return str;
}

function error_ajax(xhr, ajaxOptions, error) {
    // /hideLoader();

    // show an error
    // $('#messages').html(messageBox('Failure', 'Error', 'Ouch!'));
    $('#messages').html(messageBox('Failure', 'Error', xhr.responseText));
}

function clearMessages() {
    $('#messages').empty();
}

function messageBox(messageType, messageHeader, messageBody) {
    var messageHtml = "";
    var alertType = "";

    if (messageType == "Success") {
        alertType = "alert-success";
    }
    else if (messageType == "Error") {
        var alertType = "alert-danger";
    }
    else if (messageType == "Warning") {
        var alertType = "alert-warning ";
    }

    messageHtml = "<div id='MessageError' class='alert " + alertType + " alert-dismissible fade in' role='alert'>" +
        "<button type='button' class='close' data-dismiss='alert' aria-label='Close'>" +
        "<span aria-hidden='true'>&times;</span>" +
        "</button>" +
        "<strong style=\"font-family:Arial, Helvetica, sans-serif; font-size:15px;\">" + messageHeader + "</strong>";

    if (messageType == "Failure" || messageType == "Warning") {
        var messageWithNewLine = messageBody.replace(new RegExp('\r\n', 'g'), '<br />');
        messageHtml = messageHtml + "<br><br> <span style=\"font-family:Arial, Helvetica, sans-serif; font-size:13px;\">" + messageWithNewLine + "</span></div>";
    }
    else {
        messageHtml = messageHtml + "</div>";
    }

    return messageHtml;
}