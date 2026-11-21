"use strict";

const MochiMudClient = function (view) {
    var connection = new signalR.HubConnectionBuilder().withUrl("/MochiMudHub").build();

    connection.on("ReceiveMessage", function (message) {
        var messageDiv = document.createElement("div");
        messageDiv.classList.add('output-message');
        messageDiv.innerHTML = message;

        view.output.appendChild(messageDiv);
    });

    view.formInput.addEventListener("submit", (ev) => {
        ev.preventDefault();
        var message = textInput.value;

        connection.invoke("SendMessage", message);

        view.textInput.value = "";
        view.textInput.focus();
    });

    this.start = function () {
        connection.start().then(function () {
            console.log('connected to MochiMudHub');
        }).catch(function (err) {
            return console.error(err.toString());
        });
    }

    view.textInput.focus();
}