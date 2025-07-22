import { color } from '../config/color.js'

export class Cone {
    constructor(scene, top, height,position,name) {
        this.scene = scene;
        this.top = top;
        this.height = height;
        this.position = position;
        this.name = name;
        this.createCone({
            color: color.cone,
            height: 60,
            opacity: 0.6,
            speed: 1.0,
            position: {
                x: 8,
                y: 15,
                z: this.position.z,
            }
        })
    }

    createCone(options) {
        const geometry = new THREE.ConeGeometry(
            2,
            4,
            4,
        )

        const material = new THREE.ShaderMaterial({
            uniforms: {
                u_color: {
                    value: new THREE.Color(options.color)
                },
                u_height: this.height,
                u_top: this.top,
            },
            vertexShader: `
            uniform float u_top;
            uniform float u_height;
            
            void main() {
                float f_angle = u_height / 10.0;
                float new_x = position.x * cos(f_angle) - position.z * sin(f_angle);
                float new_y = position.y;
                float new_z = position.z * cos(f_angle) + position.x * sin(f_angle);
                
                vec4 v_position = vec4(new_x, new_y + u_top, new_z, 1.0);
                
                gl_Position = projectionMatrix * modelViewMatrix * v_position;
            }
            `,
            fragmentShader: `
            uniform vec3 u_color;
            void main() {
                gl_FragColor = vec4(u_color, 0.6);
            }
            `,
            transparent: true,
            side: THREE.DoubleSide, // 解决只显示一半的问题
            depthTest: false, // 被建筑物遮挡的问题
        })

        const mesh = new THREE.Mesh(geometry, material);
        mesh.position.copy(options.position);
        mesh.rotateZ(Math.PI);
        mesh.name = "alarmCone"

        this.scene.add(mesh);
        //console.log(mesh)
    }
}
