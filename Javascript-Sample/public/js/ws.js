import { tryParseJson } from "./config.js";

const nullFunction = () => {
    // do nothing
};

export const createWs = (url, {
    onOpen = nullFunction,
    onMessage = nullFunction,
    onError = nullFunction,
    onClose = nullFunction
}) => {

    var timeoutConnectWs = null;
    const reconnectWs = (delay = 5000) => {
        if (timeoutConnectWs) {
            clearTimeout(timeoutConnectWs);
        }

        timeoutConnectWs = setTimeout(connectWs, delay);
    }
    //connect to the local websocket to listen for skeleton information
    var ws;
    const connectWs = () => {

        if (ws) {
            ws.close();
        }

        ws = new WebSocket( url );
        ws.addEventListener('open', () => {
            onOpen();
        } );

        ws.addEventListener('error', (err) => {
            reconnectWs();
            onError();
        });

        ws.addEventListener('close', () => {
            reconnectWs();
            onClose();
        });

        ws.addEventListener('message', ({ data }) => {

            data = tryParseJson( data );
            onMessage( data );
        });

    }
    //start ws connection
    connectWs();
}