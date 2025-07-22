import { GLTFLoader } from '../../../../three/examples/jsm/loaders/GLTFLoader.js';//../../three/examples/jsm/loaders/GLTFLoader.js
import { DRACOLoader } from '../../../../three/examples/jsm/loaders/DRACOLoader.js';//../../../three/examples/jsm/loaders/DRACOLoader.js

const loader = new GLTFLoader();
const dracoLoader = new DRACOLoader();
const basePath = window.location.pathname.split('/').slice(0, -1).join('/');
dracoLoader.setDecoderPath(`${basePath}/Scripts/three/examples/jsm/libs/draco/`);
dracoLoader.preload();
console.log(dracoLoader)
//loader.setDRACOLoader(dracoLoader);//dracoLoader

export const loadGLTF = (url) => {
    return new Promise((resolve, reject) => {
        loader.load(url, (gltf) => {

            resolve(gltf.scene)
            //console.log(gltf.scene)
            // 遍历所有材质并确保它们使用正确的编码
            gltf.scene.traverse((child) => {
                if (child.isMesh) {
                    if (child.material) {
                        // 确保材质使用物理渲染
                        child.material.envMapIntensity = 1.0;
                        child.material.needsUpdate = true;

                        // 如果是标准材质或物理材质
                        if (child.material.isMeshStandardMaterial || child.material.isMeshPhysicalMaterial) {
                            child.material.roughness = 0.5;
                            child.material.metalness = 0.0;
                        }
                    }
                }
            });

            //scene.add(gltf.scene);

        }, (e) => {
            console.log('loading...')
        }, (error) => {
            reject(error)
            //console.log('failed to load model: ' + error)
        })

    })


}

