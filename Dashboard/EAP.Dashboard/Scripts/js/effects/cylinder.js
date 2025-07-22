import { color } from '../config/color.js'
import { generateAlarmTags } from '../dashboard/accbox_v3.js'


export class Cylinder {
    constructor(scene, time,text) {
        this.scene = scene;
        this.time = time;
        this.text = text;
    }

    createCylinder(options) {
        const geometry = new THREE.CylinderGeometry(
            options.radius,
            options.radius,
            options.height,
            32,
            1,
            options.open,
        )

        geometry.translate(0, options.height / 2, 0);
        const material = new THREE.ShaderMaterial({
            uniforms: {
                u_color: {
                    value: new THREE.Color(options.color)
                },
                u_height: {
                    value: options.height
                },
                u_opacity: {
                    value: options.opacity
                },
                u_speed: {
                    value: options.speed,
                },
                u_time: this.time,
            },
            vertexShader: `
        uniform float u_time;
        uniform float u_height;
        uniform float u_speed;
        
        varying float v_opacity;
        
        void main() {
          vec3 v_position = position * mod(u_time / u_speed, 1.0);
          
          v_opacity = mix(1.0, 0.0, position.y / u_height);
          
          gl_Position = projectionMatrix * modelViewMatrix * vec4(v_position, 1.0);
        }
      `,
            fragmentShader: `
        uniform vec3 u_color;
        uniform float u_opacity;
        
        varying float v_opacity;
        
        void main() {
          gl_FragColor = vec4(u_color, u_opacity * v_opacity);
        }
      `,
            transparent: true,
            side: THREE.DoubleSide, // ���ֻ��ʾһ�������
            depthTest: false, // ���������ڵ�������
        })

        const mesh = new THREE.Mesh(geometry, material);
        mesh.rotation.z = -1.5
        mesh.position.copy(options.position);
        mesh.name = 'alarmPoint'
        generateAlarmTags(mesh, this.text);
        //const tag = new CSS2DObject(div)
        this.scene.add(mesh);
    }
}
