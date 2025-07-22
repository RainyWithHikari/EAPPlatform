import * as THREE from 'three'
import { color } from '../config/color.js'

export class FlyLine{
    constructor( scene , time ){
        this.scene = scene;
        this.source = null;
        this.target = null;
        this.scene.traverse((child)=>{
            if(child.name == 'MAIN2'){
                this.source = {
                    x:child.position.x,
                    y:child.position.y +10,
                    z:child.position.z,
                }

            }else if(child.name == 'FCT201'){

                this.target = {
                    x:child.position.x,
                    y:child.position.y +10,
                    z:child.position.z,
                }

            }
        })
        this.time = time;

        this.createFlyLine({
            source:this.source,

            target:this.target,

            range:30,
            height:30,
            color:color.flyline,
            size:30,
        })

    }

    createFlyLine(options){

        //start point
        const source = new THREE.Vector3(
            options.source.x,
            options.source.y,
            options.source.z,
        )

        //end point
        const target = new THREE.Vector3(
            options.target.x,
            options.target.y,
            options.target.z,
        )

        const center = target.clone().lerp(source,0.5);

        center.y += options.height;

        //distance from start to end 
        const len = parseInt(source.distanceTo(target));
        //add bezier curve
        const curve = new THREE.QuadraticBezierCurve3(
            source,center,target
        )
        
        const points = curve.getPoints(len);

        const positions = [];
        const aPositions = []

        points.forEach((item,index) =>{
            positions.push(item.x,item.y,item.z)
            aPositions.push(index)
        })

        const geometry = new THREE.BufferGeometry();

        geometry.setAttribute('position',new THREE.Float32BufferAttribute(positions,3))
        geometry.setAttribute('a_position',new THREE.Float32BufferAttribute(aPositions,1))

        const material = new THREE.ShaderMaterial({
            uniforms:{
                u_color:{
                    value:new THREE.Color(options.color)
                },
                u_range:{
                    value:options.range
                },
                u_size:{
                    value:options.size
                },
                u_total:{
                    value:len,
                },
                u_time:this.time,
            },
            vertexShader:`
                attribute float a_position;

                uniform float u_time;
                uniform float u_size;
                uniform float u_range;
                uniform float u_total;

                varying float v_opacity;
                void main(){
                    float size = u_size;
                    float total_number = u_total*mod(u_time,1.0);

                    if(total_number>a_position && total_number < (a_position +u_range)){
                        float index = (a_position + u_range -total_number) / u_range;
                        size *= index;
                        v_opacity = 1.0;
                    }else{
                        v_opacity = 0.0;
                    }

                    gl_Position = projectionMatrix * modelViewMatrix * vec4(position,1.0);
                    gl_PointSize = size / 10.0;

                }
            `,
            fragmentShader:`
                uniform vec3 u_color;
                varying float v_opacity;

                void main(){
                    gl_FragColor = vec4(u_color,v_opacity);

                }
            `,
            transparent:true,
        })

        const point = new THREE.Points(geometry,material)
        point.name = 'flyline'

        this.scene.add(point)


    }
}