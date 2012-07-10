/// <reference path="knockout.debug.js" />
/// <reference path="jquery-1.6.4-vsdoc.js" />
/// <reference path="jquery.signalR.js" />

$(function () {

    function EchoViewModel() {

        // Data
        var self = this;
        self.message = ko.observable();
        self.messagesSent = ko.observable(3);
        self.messagesReceived = ko.observable(5);
        self.echoMessages = ko.observableArray([
            { message: "---", sender: "+++" }
        ]);
        self.heartState = ko.observable("-");

        // Behaviours
        self.sendMessage = function () {
            $.post("/api/echo", { message: self.message() }, self.messageSent);
        };

        self.messageSent = function () {
            self.messagesSent(self.messagesSent() + 1);
        };

        self.messageReceived = function (_message, _sender) {
            self.echoMessages.push({ message: _message, sender: _sender });
        };

        // Something for extra fun
        self.heartBeat = function () {
            var self = this;

            var currState = self.heartState();

            if (currState === "-")
                self.heartState("\\");
            else if (currState === "\\")
                self.heartState("|");
            else if (currState === "|")
                self.heartState("/");
            else if (currState === "/")
                self.heartState("-");
        };
    };
    var viewModel = new EchoViewModel();

    ko.applyBindings(viewModel);

    // Proxy created on the fly
    var echoHub = $.connection.echoHub;

    // Declare a function on the echoHub so the server can invoke it
    echoHub.messageUpdated = function (_message, _sender) {
        viewModel.messageReceived(_message, _sender);
    };

    echoHub.heartBeat = function () {
        viewModel.heartBeat();
    };

    // Start the connection
    $.connection.hub.start(function () {
        window.setInterval(function () {
            echoHub.doHeartBeat();
        }, 200);
    });

});