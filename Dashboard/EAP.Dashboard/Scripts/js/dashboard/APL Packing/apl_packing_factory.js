import { loadGLTF } from './utils/gltfLoader.js'
import { factoryLine } from './effects/factoryLine.js'
import { FlyLine } from './effects/flyLine.js'
import { Cone } from './effects/cone.js'
import { Circle } from './effects/circle.js'//../effects/circle.js

import { generateAlarmTags,removeMesh,getAlarmPosition,getAlarmCone,getColorofStatus,getEQProfie, getSelectEQPname, getClickModel, getDetailData, updateCameraPosition, createNamePlate, hideNamePlate } from './apl_packing.js'


export class Factory {
    constructor(scene, camera, canvas, controls, composer) {
        this.scene = scene;
        this.camera = camera;
        this.canavas = canvas;
        this.controls = controls;
        this.composer = composer;

        this.target = null;
        this.pointPosition = null;
        this.pointRotation = null;
        this.clickModel = null;
        this.formerModel = null;
        this.time = {
            value: 0,
        }
        this.top = {
            value: 0,
        }
        this.flag = false;

        this.height = {
            value: 0.1,
        }
        this.position = {
            x: 6,
            y: 8,
            z:-35
        }
        this.interval = null;

        this.loadFactory();
    }
    loadFactory() {
        const basePath = window.location.pathname.split('/').slice(0, -1).join('/');
        //'../../Content/Scenes/accline-0731-processed.glb'
        loadGLTF(`${basePath}/Content/Scenes/test.glb`).then(object => {

            object.traverse((child) => {
                if (child.isMesh) {
                    //console.log(child.material);
                    new factoryLine(this.scene, child, this.time)
                    
                    //new Font(this.scene, child)


                }
            })
            //new Background(this.scene);

            
            this.addClick();
            this.addWheel();
            //new FlyLine(this.scene, this.time)
            ////new Circle(this.scene, this.time)
            
            this.ChartssetInterval(this.scene, this.top, this.height)
            ////this.initEffect();
            //getColorofStatus();
           

        });

    }

    addClick() {

        let flag = true;
        this.canavas.onmousedown = () => {
            flag = true;
            this.canavas.onmousemove = () => {
                flag = false;
            }

        }
        this.canavas.onmouseup = (event) => {
            if (flag) {
                
                this.clickEvent(event)
            }
            this.canavas.onmousemove = null;
        }

    }
    addWheel() {
        this.canavas.onmousewheel = (event) => {
            const value = 30;
            const x = ((event.clientX - this.canavas.getBoundingClientRect().left) / this.canavas.clientWidth) * 2 - 1;
            const y = -((event.clientY - this.canavas.getBoundingClientRect().top) / this.canavas.clientHeight) * 2 + 1;
            const vector = new THREE.Vector3(x, y, 0.5);

            vector.unproject(this.camera)
            vector.sub(this.camera.position).normalize()

            if (event.wheelDelta > 0) {
                this.camera.position.x += vector.x * value;
                this.camera.position.y += vector.y * value;
                this.camera.position.z += vector.z * value;

                this.controls.target.x += vector.x * value;
                this.controls.target.y += vector.y * value;
                this.controls.target.z += vector.z * value;

            } else {
                this.camera.position.x -= vector.x * value;
                this.camera.position.y -= vector.y * value;
                this.camera.position.z -= vector.z * value;

                this.controls.target.x -= vector.x * value;
                this.controls.target.y -= vector.y * value;
                this.controls.target.z -= vector.z * value;
            }
        }
    }

    clickEvent(event) {

        const x = ((event.clientX - this.canavas.getBoundingClientRect().left) / this.canavas.clientWidth) * 2 - 1;
        const y = -((event.clientY - this.canavas.getBoundingClientRect().top) / this.canavas.clientHeight) * 2 + 1;

        const standardVector = new THREE.Vector3(x, y, 0.5);
        //console.log(standardVector)
        const worldVector = standardVector.unproject(this.camera);

        

        const ray = worldVector.sub(this.camera.position).normalize();

        const raycaster = new THREE.Raycaster(this.camera.position, ray);
        //const mousePosition = raycaster.intersectObjects(this.scene);
        
        const intersects = raycaster.intersectObjects(this.scene.children, true);

        let point3d = null;
        if (intersects.length) {
            point3d = intersects[0];
            //console.log(intersects[0])
        }
        if (point3d && (point3d.object.name.startsWith('MAIN') || point3d.object.name.startsWith('FCT'))) {//point3d.object.name != 'gridHelper' && point3d.object.name != 'flyLine' && !point3d.object.name.includes('tag')) {
            this.formerModel = this.clickModel
            this.clickModel = point3d.object;
            if (!this.clickModel.visible) {

                this.resetMaterialsVisible();
                
                this.clickModel = null;
                getClickModel(this.clickModel, this.formerModel)
                var overAllpreview = new THREE.Vector3(point3d.object.position.x + 40, point3d.object.position.y+10, point3d.object.position.z);
                updateCameraPosition(overAllpreview)
                hideNamePlate()
                removeMesh('alarmPoint')
                removeMesh('labelLine')
                removeMesh('alarmLabel')
                return;
            }
            

            window.eqid = point3d.object.name;
            getEQProfie(point3d.object.name)//.then(createNamePlate(window.raderArr))
            createNamePlate(point3d.object.name) 
            
            getDetailData(point3d.object.name,window.datetime,'Default')
            getSelectEQPname()

            
            
            updateCameraPosition(point3d.object.position)
            this.resetMaterialsInvisible()
            getClickModel(this.clickModel, this.formerModel)
            this.formerModel = this.clickModel;

            this.handleAlarmPosition(this.scene, this.time)
            
                    //console.log(positionArr)
                    //if (positionArr) {
                    //    positionArr.forEach(item => {
                    //        console.log(item)
                    //        new Circle(this.scene, this.time, item)
                    //    })
                    //} 

                
            


        } else {
            this.resetMaterialsVisible();
            getClickModel(null, this.formerModel)
            hideNamePlate()
            removeMesh('alarmPoint')
            removeMesh('labelLine')
            removeMesh('alarmLabel')
        }

    }
    resetMaterialsInvisible() {
        this.scene.traverse(obj => {
            //&& !obj.name.startsWith('tag') && obj.name != 'flyline'
            if (obj.isMesh && obj != this.clickModel  && obj.name != 'background'  && obj.name != 'gridHelper') {
                obj.visible = false;
                
            }
            //obj.material.emissive.setHex(0x000000)
        })
        removeMesh('alarmCone')
        clearInterval(this.interval)
        
    }
    resetMaterialsVisible() {
        this.scene.traverse(obj => {

            if (obj.isMesh) {
                obj.visible = true;

            }
            //obj.material.emissive.setHex(0x000000)
        })
        this.ChartssetInterval(this.scene, this.top, this.height)
    }
    ChartssetInterval(scene, top, height) {

        this.interval = setInterval(function () {
            
            removeMesh('alarmCone')
            var alarmMeshs = getAlarmCone();
            if (alarmMeshs) {
                alarmMeshs.forEach(item => {
                    //new Cone(scene, top, height, item.position)
                })
            }
        }, 1500);
    }
    handleAlarmPosition(scene,time) {
        getAlarmPosition().then(function (response) {
            console.log(response)
            response.forEach(item => {

                new Circle(scene, time, item)
            })
        }).catch(function (error) {
            console.log(error)
        })
    }
    
    start(delta) {

        TWEEN.update();
        
        this.controls.update();
        this.composer.render();


        this.time.value += delta;
        this.height.value += 0.1;
        if (this.height.value > 3) {
            this.height.value = 0.1;
        }
        if (this.top.value > 1 || this.top.value < 0) {
            this.flag = !this.flag;
        }

        this.top.value += this.flag ? -0.1 : 0.1;
        
        

    }
}