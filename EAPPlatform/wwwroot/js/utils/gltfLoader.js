import { GLTFLoader } from '../../three/examples/jsm/loaders/GLTFLoader.js';
import { DRACOLoader } from '../../three/examples/jsm/loaders/DRACOLoader.js';

const loader = new GLTFLoader();
const dracoLoader = new DRACOLoader();
dracoLoader.setDecoderPath('../../three/examples/jsm/libs/draco/');
dracoLoader.preload();
loader.setDRACOLoader(dracoLoader);

export const loadGLTF = (url) => {
    return new Promise((resolve, reject) => {
        loader.load(url, (gltf) => {

            resolve(gltf.scene)
            //console.log(gltf.scene)

        }, (e) => {
            console.log('loading...')
        }, (error) => {
            reject(error)
            //console.log('failed to load model: ' + error)
        })

    })


}

