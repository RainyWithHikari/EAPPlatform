import { color } from '../../../config/color.js'
import { Font } from './font.js'
export class factoryLine {
    constructor(scene, child, time) {
        this.scene = scene;
        this.child = child;
        this.time = time;

        this.createMesh();
        
        //this.createMeshTest();
        //this.createLine();

    }
    computedMesh() {

        this.child.geometry.computeBoundingBox();
        this.child.geometry.computeBoundingSphere();
    }
    createMeshTest() {
        this.computedMesh();
        const { max, min } = this.child.geometry.boundingBox
        const size = max.x - min.x;

        const vertexShader = `
        varying vec3 v_Normal;
        void main(){
            v_Normal = normal;
            gl_Position = projectionMatrix * modelViewMatrix * vec4(position ,1.0);
        }
        `;
        const fragmentShader = `
        varying vec3 v_Normal;
        uniform vec3 u_factory_color;
        uniform float u_Metalness;
        void main() {
            vec3 lightDirection = normalize(vec3(2.0, 2.0, 0.0)); // 光源方向
            float brightness = clamp(dot(v_Normal, lightDirection), 0.0, 1.0); // 点积计算明暗度
            vec3 diffuse = u_factory_color; //模型颜色作为漫反射
            vec3 specular = vec3(1.0);//镜面反射颜色为黑色
            vec3 finalColor = mix(diffuse,specular,u_Metalness);
            gl_FragColor = vec4(finalColor*brightness, 1.0); // 颜色值使用明暗度
        }
        `;


        const material_old = new THREE.ShaderMaterial({
            uniforms: {
                u_factory_color: {
                    value: new THREE.Color(color.mesh)
                },
                u_head_color: {
                    value: new THREE.Color(color.head)
                },
                u_size: {
                    value: size
                }
            },
            vertexShader: `
            varying vec3 v_position;
            void main(){
                v_position = position;

                gl_Position = projectionMatrix * modelViewMatrix * vec4(position ,1.0);
            }`,
            fragmentShader: `
            varying vec3 v_position;

            uniform vec3 u_factory_color;
            uniform vec3 u_head_color;
            uniform float u_size;

            void main(){
                vec3 base_color = u_factory_color;
                //base_color = mix(base_color , u_head_color , v_position.x / u_size);
                base_color = mix(base_color , u_head_color , v_position.x / u_size);

                gl_FragColor = vec4(base_color,1.0);
            }`,
        })

        const material = new THREE.ShaderMaterial({
            uniforms: {
                u_factory_color: {
                    value: new THREE.Color(color.mesh)
                },
                u_Metalness: { value: 0.0 },
            },
            vertexShader,
            fragmentShader,


        })
        const mesh = new THREE.Mesh(this.child.geometry, material_old)
        console.log(mesh);

        mesh.position.copy(this.child.position)
        mesh.rotation.copy(this.child.rotation)
        mesh.scale.copy(this.child.scale)


        this.scene.add(mesh);

    }
    createLine() {
        //get bounderary
        const geometry = new THREE.EdgesGeometry(this.child.geometry)
        //const material = new THREE.LineBasicMaterial({color:})
        const { max, min } = this.child.geometry.boundingBox

        const material = new THREE.ShaderMaterial({
            uniforms: {
                line_color: {
                    value: new THREE.Color(color.soundLine),
                },
                //contineous changing value: u_time
                u_time: this.time,
                //position of scan ray
                u_max: {
                    value: max
                },
                u_min: {
                    value: min
                },
                //color of scan ray
                live_color: {
                    value: new THREE.Color(color.liveColor)
                }



            },
            vertexShader: `
            uniform float u_time;
            uniform vec3 live_color;
            uniform vec3 line_color;
            uniform vec3 u_max;
            uniform vec3 u_min;
            
            varying vec3 v_color;


            void main(){
                float new_time = mod(u_time * 0.1 , 1.0);
                float rangeY = mix(u_min.z,u_max.z,new_time);

                if(rangeY < (position.z +200.0) && rangeY >(position.z -200.0)){
                    float f_index = 1.0 -sin((position.z -rangeY)/200.0 *3.14);
                    float r = mix(live_color.r,line_color.r,f_index);
                    float g = mix(live_color.g,line_color.g,f_index);
                    float b = mix(live_color.b,line_color.b,f_index);
                    v_color = vec3(r,g,b);
                }else{
                    v_color = line_color;
                }
                gl_Position = projectionMatrix * modelViewMatrix * vec4(position,1.0);

            }
            `,
            fragmentShader: `
            varying vec3 v_color;

            void main(){
                gl_FragColor = vec4(v_color,1.0);

            }
            `,
            depthTest: false,
        })

        //create line
        const line = new THREE.LineSegments(geometry, material)

        //console.log(material.vertexShader)



        //inhirte rotation
        line.scale.copy(this.child.scale)
        line.position.copy(this.child.position)
        line.rotation.copy(this.child.rotation)
        this.scene.add(line)


    }
    createMesh() {

        const material = new THREE.MeshStandardMaterial({
            color: new THREE.Color(color.mesh),
            roughness: 0.0, // 设置粗糙度
            metalness: 0, // 设置金属度
            flatShading: false,
        });
        const mesh = new THREE.Mesh(this.child.geometry, material)
        //const mesh = new THREE.Mesh(this.child.geometry, this.child.material)


        mesh.position.copy(this.child.position)
        mesh.rotation.copy(this.child.rotation)
        mesh.scale.copy(this.child.scale)
        if (this.child.name == 'MAIN81') {
            mesh.name = 'MAIN8.1'
        }
        else if (this.child.name == 'MAIN82') {
            mesh.name = 'MAIN8.2'
        }
        else if (this.child.name == 'MAIN91') {
            mesh.name = 'MAIN9.1'
        }
        else {
            mesh.name = this.child.name
        }

        this.scene.add(mesh);

        new Font(this.scene, mesh)

    }


}