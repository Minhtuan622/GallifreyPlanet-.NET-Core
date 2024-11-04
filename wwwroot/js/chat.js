"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

$("#sendButton").prop("disabled", true); 

connection.on("ReceiveMessage", function (sender, message) {
    $("#messagesList").append($("<li>").text(`${sender} says: ${message}`));
});

connection.start().then(function () {
    $("#sendButton").prop("disabled", false); 
}).catch(function (err) {
    console.error(err.toString());
});

$("#sendButton").on("click", function (event) {
    connection.invoke(
        "SendMessage",
        $("#chatIdInput").val(),
        $("#senderIdInput").val(),
        $("#receiverIdInput").val(),
        $("#messageInput").val()
    ).catch(function (err) {
        console.error(err.toString());
    });

    event.preventDefault();
});