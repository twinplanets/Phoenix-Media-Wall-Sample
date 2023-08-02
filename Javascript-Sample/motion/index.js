const path = require('path');
const { createServer } = require('http');
const fs = require('fs');
const { WebSocketServer } = require('ws');

const server = createServer({});

var [command,file,input='full-1',port=8080] = process.argv;
var pathInput = path.resolve( __dirname,`./data/${input}.txt` );

if( !fs.existsSync( pathInput ) ){
  console.log(`Input "${input}" does not exist`);
}else{
  console.log(`Running motion data "${input}"`);

  var stream = fs.createReadStream( pathInput, {autoClose:true } );
  
  const rl = require('readline').createInterface({
    input : stream,
    crlfDelay : Infinity
  });
  
  var lines = [], lineIndex = 0;
  rl.on('line', data => {
    
    if( data.indexOf("[]") > -1 ){
      return;
    }
  
    if( !data.indexOf("[") ){
      return;
    }
  
    try{
      data = JSON.parse( data );
    
      for( var i in data ){
        var item = data[i];
        item.line = lines.length;
        // console.log( item, lines.length );
      }
      lines.push( JSON.stringify(data) );
    }catch( err ){
  
    }
  
  } );
  
  const notifyListeners = ( data ) => {
  
    if( data && data.length ){
      // console.log( lineIndex, data );
      wss.clients.forEach( client => {
          client.send( data, {isBinary:false} );
      } );
    }
  }
  
  setInterval( () => {
    
    notifyListeners( lines[ (lineIndex++) % lines.length ] );
  
  }, 200 );
  
  const wss = new WebSocketServer({
    //port: 8080,
    server, 
    perMessageDeflate: {
      zlibDeflateOptions: {
        // See zlib defaults.
        chunkSize: 1024,
        memLevel: 7,
        level: 3
      },
      zlibInflateOptions: {
        chunkSize: 10 * 1024
      },
      // Other options settable:
      clientNoContextTakeover: true, // Defaults to negotiated value.
      serverNoContextTakeover: true, // Defaults to negotiated value.
      serverMaxWindowBits: 10, // Defaults to negotiated value.
      // Below options specified as default values.
      concurrencyLimit: 10, // Limits zlib concurrency for perf.
      threshold: 1024 // Size (in bytes) below which messages
      // should not be compressed if context takeover is disabled.
    }
  });
  
  wss.on('connection', ( client ) => {
      console.log('connected');
      client.send("[]");
  })
  

  server.listen( port, ( err, result ) => {
    console.log(`Listening on port "${port}"`);
  } );
}

