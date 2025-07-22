import * as THREE from 'three'
import { FontLoader } from '../../three/examples/jsm/loaders/FontLoader.js'
import { TextGeometry } from '../../three/examples/jsm/geometries/TextGeometry.js'
import { eqpNames } from '../config/EquipmentName.js'

export class Font {
    constructor(scene,child) {
        this.scene = scene;
        this.child = child;
       
        this.font = null;
        this.showName = null;
        this.init();
    }
    init() {
        const loader = new FontLoader();
        loader.load('../../js/config/font.json', (font) => {
            this.font = font;
            this.showName = eqpNames.find(item => item.EQID == this.child.name)
            //console.log(this.showName)
            this.createTextQueue();
            this.createNameTags();
            //console.log(eqpNames.find(item => item.EQID === this.child.name))
        })
    }

    createTextQueue() {

        [{
            text: this.child.name,//this.showName.Name,//this.child.name, //eqpNames.find(item => item.EQID === this.child.name).Name,//
            size: 1,
            position: {
                x: 10,
                y: 15,
                z: this.child.position.z+2,
            },
            rotate: 1.5,
            color: '#ffffff',
            id: this.child.name
        }].forEach(item => {
            this.createText(item);
        })
    }

    createNameTags() {

        [{
            text: this.showName.Name,//this.child.name, //eqpNames.find(item => item.EQID === this.child.name).Name,//
            size: 1,
            position: {
                x: 10,
                y: 13,
                z: this.child.position.z+2,
            },
            rotate: 1.5,
            color: '#ffffff',
            id: this.child.name
        }].forEach(item => {
            this.createNameTagText(item);
        })
    }

    createText(data) {
        const geometry = new TextGeometry(data.text, {
            font: this.font,
            size: 1.1,
            height: 0.3,
        })

        const material = new THREE.ShaderMaterial({
            uniforms: {
                color: { value: new THREE.Color(0xffffff) } // 初始颜色为白
            },
            vertexShader: `
                void main(){
                    gl_Position = projectionMatrix * modelViewMatrix * vec4(position ,1.0);
                }
            `,
            fragmentShader: `
            uniform vec3 color;
                void main(){
                    vec3 base_color = color;
                    gl_FragColor = vec4(base_color,1.0);
                }
            `,
            depthTest: false, // 被建筑物遮挡的问题
        })

        const mesh = new THREE.Mesh(geometry, material)
        mesh.position.copy(data.position)
        mesh.rotateY(data.rotate)
        //console.log(data.id)
        mesh.name = 'tag_' + data.id;
        //console.log(mesh)
        this.scene.add(mesh)
    }

    createNameTagText(data) {
        const geometry = new TextGeometry(data.text, {
            font: this.font,
            size: 0.8,
            height: 0.3,
        })

        const material = new THREE.ShaderMaterial({
            uniforms: {
                color: { value: new THREE.Color(0xffffff) } // 初始颜色为白
            },
            vertexShader: `
                void main(){
                    gl_Position = projectionMatrix * modelViewMatrix * vec4(position ,1.0);
                }
            `,
            fragmentShader: `
            uniform vec3 color;
                void main(){
                    vec3 base_color = color;
                    gl_FragColor = vec4(base_color,1.0);
                }
            `,
            depthTest: false, // 被建筑物遮挡的问题
        })

        const mesh = new THREE.Mesh(geometry, material)
        mesh.position.copy(data.position)
        mesh.rotateY(data.rotate)
        //console.log(data.id)
        mesh.name = 'tagName_' + data.id;
        //console.log(mesh)
        this.scene.add(mesh)
    }

}