(function (signalRHelper, $, undefined) {
    var connected = false;
    var hubUrl;
    var hubName;
    var actions = [];
    var monitorService = false;
    var preStartRetryEnabled = false;
    var serviceRunningCallback;
    var serviceNotRunningCallback;
    var enableJsonP = false;
    var myHub;

    signalRHelper.enableServiceMonitor = function (serviceNotRunningAction, serviceRunningAction) {
        monitorService = true;

        serviceNotRunningCallback = serviceNotRunningAction;
        serviceRunningCallback = serviceRunningAction;

        return this;
    };

    signalRHelper.server = function () {
        return myHub.server;
    };

    signalRHelper.addAction = function (name, action) {
        var item = { name: name, action: action };
        actions.push(item);
        return this;
    };

    signalRHelper.configureSignalRHub = function (url, name) {
        hubUrl = url;
        hubName = name;
        return this;
    };

    signalRHelper.run = function () {
        tryStartSignalR();
        return this;
    };
    
    signalRHelper.enablePreStartRetry = function () {
        preStartRetryEnabled = true;
        return this;
    };

    signalRHelper.enableJsonP = function () {
        enableJsonP = true;
        return this;
    };

    function delay(callback) {
        var millisecondsToWait = 10000;
        setTimeout(function () {
            callback();
        }, millisecondsToWait);
    }

    function tryStartSignalRAsync(func) {
        var loop = {
            next: function () {
                if (connected) {
                    return;
                }

                if (!connected) {
                    func(loop);
                }
            }
        };
        loop.next();
        return loop;
    }

    function updateStatus(running) {
        if (!monitorService)
            return;
        if (running)
            serviceRunningCallback();
        else
            serviceNotRunningCallback();
    }

    function tryStartSignalR() {

        $.getScript(hubUrl + "/signalr/hubs")
            .done(function () {
                runSignalR();
            })
            .fail(function () {
                if (!preStartRetryEnabled)
                    return;
                tryStartSignalRAsync(function (loop) {
                    delay(function () {
                        if (!connected) {
                            tryStartSignalRWithoutFail(hubUrl);
                            loop.next();
                        } else {
                            updateStatus(true);
                        }
                    });
                }
                );
            });
    }

    function runSignalR() {
        $.connection.hub.url = hubUrl + "/signalr";

        var connection = $.connection.hub;
        myHub = connection.createHubProxy(hubName);

        if (myHub != undefined) {

            for (var i = 0; i < actions.length; i++) {
                myHub.on(actions[i].name, actions[i].action);
            }

            $.connection.hub.start({ jsonp: enableJsonP }).done(function () {
                connected = true;
                updateStatus(true);
            });

            $.connection.hub.reconnected(function () {

                updateStatus(true);
            });

            $.connection.hub.reconnecting(function () {
                updateStatus(false);

                setTimeout(function () {
                    $.connection.hub.start();
                }, 5000);
            });

        }
    }

    function tryStartSignalRWithoutFail() {

        $.getScript(hubUrl + "/signalr/hubs")
            .done(function () {
                runSignalR();
            });
    }

}(window.signalRHelper = window.signalRHelper || {}, jQuery))



