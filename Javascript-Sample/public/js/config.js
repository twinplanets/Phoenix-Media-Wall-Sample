export const parseQueryString = () => {
    var params = new URLSearchParams( window.location.search );

    const keys = params.keys();
    const settings = {};

    for (const key of params.keys()) {
        settings[key] = tryParseJson( params.get( key ) );
    }

    return settings;
}

export const tryParseJson = ( data ) => {
    try{
        return JSON.parse( data );
    }catch( err ){
    }
    
    return data;
}