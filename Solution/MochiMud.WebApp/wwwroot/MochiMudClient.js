"use strict";

const MochiMudClient = function (outputElement) {    
    var connection = new signalR.HubConnectionBuilder().withUrl("/MochiMudHub").build();

    connection.on("ReceiveMessage", function (message) {
        var messageDiv = document.createElement("div");
        messageDiv.classList.add('output-message');
        messageDiv.textContent = message;
        outputElement.appendChild(messageDiv);
    });

    this.start = function () {
        connection.start().then(function () {
            console.log('connected to MochiMudHub');
        }).catch(function (err) {
            return console.error(err.toString());
        });
    }
}

var mochiMudClient = new MochiMudClient(document.getElementById('output'));
mochiMudClient.start();