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
            // �������в��ʲ�ȷ������ʹ����ȷ�ı���
            gltf.scene.traverse((child) => {
                if (child.isMesh) {
                    if (child.material) {
                        // ȷ������ʹ��������Ⱦ
                        child.material.envMapIntensity = 1.0;
                        child.material.needsUpdate = true;

                        // ����Ǳ�׼���ʻ��������
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

