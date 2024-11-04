"use strict";

const connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
const sendBtn = $("#sendButton");

sendBtn.prop("disabled", true); 

connection.on("ReceiveMessage", function (sender, message) {
    $("#messagesList").append($("<li>").text(`${sender} says: ${message}`));
});

connection.start().then(function () {
    $("#sendButton").prop("disabled", false); 
}).catch(function (err) {
    console.error(err.toString());
});

sendBtn.on("click", function (event) {
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