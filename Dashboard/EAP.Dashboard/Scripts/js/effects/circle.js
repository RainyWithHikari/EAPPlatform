import { Cylinder } from './cylinder.js'
import { color } from '../config/color.js'

export class Circle {
    constructor(scene, time,obj) {
        this.config = {
            radius: 0.5,
            color: color.circle,
            opacity: 0.6,
            height: 0.5,
            open: false,
            position: {
                x: obj.position.x,
                y: obj.position.y,
                z: obj.position.z,
            },
            speed: 0.8,
        }

        new Cylinder(scene, time, obj.text).createCylinder(this.config);
    }
}