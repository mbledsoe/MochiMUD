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

const MochiMudClient = function (view) {
    var connection = new signalR.HubConnectionBuilder().withUrl("/MochiMudHub").build();

    connection.on("ReceiveMessage", function (message) {
        var messageDiv = document.createElement("div");
        messageDiv.classList.add('output-message');
        messageDiv.innerHTML = message;

        view.output.appendChild(messageDiv);
        scrollDown(view.output);
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

    resizeUI(view);

    window.addEventListener('resize', () => {
        resizeUI(view);
    });

    view.textInput.focus();
}