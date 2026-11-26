"use strict";

function scrollDown(element) {
    element.scrollTop = element.scrollHeight;
}

function resizeUI(view) {
    var uiHeight = document.documentElement.clientHeight;
    view.ui.style.padding = '20px 0px';
    view.ui.style.height = `${uiHeight}px`;
    scrollDown(view.output);
}

function createOutputMessage(message) {
    var messageDiv = document.createElement("div");
    messageDiv.classList.add('output-message');
    messageDiv.innerHTML = message;

    return messageDiv;
}

function outputMessage(message, output) {
    output.appendChild(createOutputMessage(message));
    scrollDown(view.output);
}

function sendMessage(textInput, connection) {
    var message = textInput.value;
    connection.invoke("SendMessage", message);
    view.textInput.value = "";
    view.textInput.focus();
}

const MochiMudClient = function (view) {
    var connection = new signalR.HubConnectionBuilder().withUrl("/MochiMudHub").build();

    connection.on("ReceiveMessage", function (message) {
        outputMessage(message, view.output);
    });

    view.formInput.addEventListener("submit", (ev) => {
        ev.preventDefault();
        sendMessage(view.textInput, connection);
    });

    this.start = function () {
        connection.start().then(function () {
            console.log('connected to MochiMudHub');
        }).catch(function (err) {
            return console.error(err.toString());
        });
    }

    window.addEventListener('resize', () => {
        resizeUI(view);
    });

    resizeUI(view);
    view.textInput.focus();
}