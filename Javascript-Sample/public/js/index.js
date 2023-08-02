import { OrbitControls } from "./orbit.js";
import { parseQueryString } from "./config.js";
import { 
    SPINE1,SPINE2,SPINE3,NECK,NOSE,
    LEFT_SHOULDER,LEFT_ELBOW,LEFT_WRIST,LEFT_HIP,LEFT_KNEE,LEFT_ANKLE,
    RIGHT_SHOULDER,RIGHT_ELBOW,RIGHT_WRIST,RIGHT_HIP,RIGHT_KNEE,RIGHT_ANKLE,
} from "./joints.js";
import { createWs } from "./ws.js";

const config = parseQueryString();
const {wsDomain='localhost',wsPort=8080} = config || {};

//set up the basic scene
const scene = new THREE.Scene();
const camera = new THREE.PerspectiveCamera( 75, window.innerWidth / window.innerHeight, 0.1, 1000 );
camera.position.z = 50;

//add the renderer (canvas)
const renderer = new THREE.WebGLRenderer();
renderer.setSize( window.innerWidth, window.innerHeight );
document.body.appendChild( renderer.domElement );

//add orbit controls to allow developer to navigate and explore scene
const controls = new OrbitControls( camera, renderer.domElement );

//IMPORTANT - create a container we can add our items to - this can be easily rotated and mirrored without making the rest of our code more complex
var container = new THREE.Group();
scene.add( container );
container.position.y = -20;

//start the render loop
function animate() {
    requestAnimationFrame( animate );
    controls.update();
    renderer.render( scene, camera );
}
animate();

//listen to window resize
window.addEventListener( 'resize', onWindowResize, false );

function onWindowResize(){

    camera.aspect = window.innerWidth / window.innerHeight;
    camera.updateProjectionMatrix();

    renderer.setSize( window.innerWidth, window.innerHeight );
}

// COLORS for debuggging
const COLORS = [
    0xff0000,0xffff00,0xffffff,
    0x00ff00,0x00ffff,
    0x0000ff,0xff00ff
]

// SCALES DOWN ALL JOINTS POSITIONS
const SCALE_JOINTS = new THREE.Vector3(.1,.1,.1);
// OFFSETS THE ROOT POSITION OF THE SKELETON
const ORIGIN_BODY = new THREE.Vector3(0,20,0);
// SCALES DOWN THE ROOT POSITION OF THE SKELETON
// NOTE: this example limits the motion in the y and z axis, limits some up down and back/forth movement 
const SCALE_BODY = new THREE.Vector3(.1,.5,.1);


const DEBUG_JOINTS = true;
const DEBUG_STAGE = true;

const boneName = ( jointIndex = -1 ) => jointIndex >= 0 ? `b${jointIndex}` : null;
const jointName = ( jointIndex = -1 ) => jointIndex >= 0 ? `j${jointIndex}` : null;

//utilities for building our skeleton
const createCube = ( {name, depth = 1, width = 1, height = 1, color = 0x00ff00, opacity = 1 } = {} ) => {
    
    const geometry = new THREE.BoxGeometry( width, height, depth );
    const material = new THREE.MeshBasicMaterial( { color: color, transparent: opacity < 1 ? true : false, opacity } );
    const mesh = new THREE.Mesh( geometry, material );

    mesh.name = name;
    
    return mesh;
}

const createBone = ( {name,color = 0x00ff00, ...props } = {} ) => {
    var bone = new THREE.Group();
    bone.name = name;

    var cube = createCube({depth:1,width:1,height:1,color,...props });
    bone.add( cube );
    cube.position.z = .5;
    return bone;
}

var SkeletonIndex = 0;
const createSkeleton = () => {

    var color = COLORS[ SkeletonIndex++ % COLORS.length ];

    const buildBone = ( config, parent ) => {
        parent = parent || new THREE.Group();

        const name = config.name || boneName(config.origin);
        //create the bone and add it to the parent
        const bone = createBone( {...config,name,color} );
        bone.userData = config;
        parent.add( bone );

        //iterate over all the children as well
        if( config.children ){
            config.children.forEach( ( child ) => {
                //add the child to this bone
                buildBone( child, bone );
            } )
        }

        return parent;
    };

    const buildBones = ( bones, parent ) => {
        parent = parent || new THREE.Group();

        bones.forEach( ( child ) => {
            //add the child to this bone
            buildBone( child, parent );
        } );

        return parent;
    }


    
    //recursive loop to build the full heirachy of bones
    const skeleton = buildBones([
        {
            origin:SPINE1,
            target:SPINE2,
        },
        {
            origin:SPINE2,
            target:SPINE3,
        },
        {
            origin:NECK,
            target:NOSE,
        },
        {
            origin:RIGHT_SHOULDER,
            target:RIGHT_ELBOW,
        },
        {
            origin:RIGHT_ELBOW,
            target:RIGHT_WRIST,
        },
        {
            origin:RIGHT_HIP,
            target:RIGHT_KNEE,
        },
        {
            origin:RIGHT_KNEE,
            target:RIGHT_ANKLE,
        },
        {
            origin:LEFT_SHOULDER,
            target:LEFT_ELBOW,
        },
        {
            origin:LEFT_ELBOW,
            target:LEFT_WRIST,
        },
        {
            origin:LEFT_HIP,
            target:LEFT_KNEE,
        },
        {
            origin:LEFT_KNEE,
            target:LEFT_ANKLE,
        },
    ]);

    if( DEBUG_JOINTS ){
        color = COLORS[ SkeletonIndex++ % COLORS.length ];
        //add points for each of the joints
        for( var i = 0; i < 27; i++ ){
            var size = i === 0 ? 4 : 2;

            var joint = createCube({
                name: `j${i}`,
                width: size,
                height: size,
                depth: size,
                color,
                opacity: 0.5
            });

            skeleton.add( joint );
        }
    }

    return skeleton;
}

// a utility method that will take a position and offset and scale it
const applyTransform = ( target, position, {origin,scale} = {} ) => {
    if( target ){
        target.copy( position );
        
        if( scale ){
            target.multiply( scale );
        }

        if( origin ){
            target.sub( origin );
        }
    }

    return target;
}

// a convenience method that wraps applyTransform, it checks that the object exists before setting the position
const applyPosition = ( target, position, {origin,scale} = {} ) => {
    //apply the transform to the target position
    if( target ){
        applyTransform( target.position, position, {origin,scale} );
    }
}

const applyDirection = ( target, position, {origin,scale} = {} ) => {
    //apply the transform to the target position
    
    if( target ){
        //what do we need our object to lookAt
        var focus = applyTransform( new THREE.Vector3(), position, {origin,scale} );
        //lookAt requires the world position - our position is relative to the parent
        target.lookAt( target.parent.localToWorld( focus ) );
    }
}


var skeletons = {};

createWs(
    `ws://${wsDomain}:${wsPort}`,
    {
        onOpen : () => {
            console.log('ws:open');
        },
        onError : ( err ) => {
            console.log('ws:error',err);
        },
        onClose : () => {
            console.log('ws:close');
        },
        onMessage : ( data ) => {
            try{
                data.forEach( item => {
                    //id if the user is on the position
                    var id = item.position.id;
                    //determine if we need to create a new skeleton or use an existing one
                    var skeleton = skeletons[id] = skeletons[id] || createSkeleton();
                    container.add( skeleton );
        
                    //update the position of the skeleton
                    applyPosition( skeleton, item.position, {origin:ORIGIN_BODY,scale:SCALE_BODY} );
                    // console.log('copying', skeleton.position);
                    skeleton.updateWorldMatrix( true );
    
                    //position the bones
                    var from = new THREE.Vector3();
                    var to = new THREE.Vector3();
                    item.joints.forEach( (joint, index ) => {
                        // console.log( index );
                        var b = skeleton.getObjectByName( boneName( index ) );
                        if( b ){
                            //apply the position data to our "from" value so we can reuse the data
                            applyTransform(from, joint, {scale:SCALE_JOINTS} );
                            //copy the cached value to the bone
                            b.position.copy( from );
    
                            var target = b.userData.target ? item.joints[ b.userData.target ] : null;
                            if( target ){
    
                                //apply the position data to our "to" value so we can reuse the data
                                
                                applyTransform(to, target, {scale:SCALE_JOINTS} );
                                //apply the rotation by looking at the target 
                                applyDirection(b, target, {scale:SCALE_JOINTS} );
                                //scale bone length to match the distance between from and to
                                b.scale.z = from.distanceTo( to );
                                // console.log( target,joint, from, to, from.distanceTo( to ) );
                                //scale is 
                            }
                        }
                    } )
    
    
                    // IF DEBUGGING POSITION ALL THE JOINTS DIRECTLY
                    if( DEBUG_JOINTS ){
                        item.joints.forEach( (joint, index ) => {
                            // console.log( index );
                            var j = skeleton.getObjectByName( jointName( index ) );
                            if( j ){
                                applyPosition(j, joint, {scale:SCALE_JOINTS});
                            }
                        } )
                    }
        
                    //flag the skeleton as recently updated
                    skeleton.userData.updatedAt = Date.now();
                } );
        
        
                //iterate over the skeletons and remove any that are dead
                for( var id in skeletons ){
                    if( skeletons[id].userData.updatedAt < (Date.now() - 1000) ){
                        //this isn't very efficient - consider caching the removed skeleton and reusing later
                        skeletons[id].parent.remove( skeletons[id] );
                        delete skeletons[id];
                        console.log('removed skeleton');
                    }
                }
            }catch( err ){
                console.error('Error parsing skeleton data', err );
            }
        }
    }
)

if( DEBUG_STAGE ){
    const stage = createCube({width:1000,depth:1000,height:0.1,color:0xff0000,opacity:0.2});
    const line = createCube({width:1000,depth:1,height:0.1,color:0xffffff,opacity:0.2});
    stage.add( line );
    container.add( stage );
}