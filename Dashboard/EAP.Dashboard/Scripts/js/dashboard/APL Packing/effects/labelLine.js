
export class LabelLine {
    constructor(scene, positionStart, positionEnd) {
        this.scene = scene;
        this.positionStart = positionStart;
        this.positionEnd = positionEnd;
        this.init()
    }
    init() {
        var lineGeometry = new THREE.BufferGeometry();
        var positions = [
            this.positionStart.x, this.positionStart.y, this.positionStart.z,
            this.positionEnd.x, this.positionEnd.y, this.positionEnd.z
        ];

        var positionsAttribute = new THREE.Float32BufferAttribute(positions, 3);
        lineGeometry.setAttribute('position', positionsAttribute);

        var lineMaterial = new THREE.LineBasicMaterial({
            color: 0xff0000,
            dashSize: 1,
            gapSize: 1,
            transparent: true,
            opacity: 0.5,
            depthTest: false
        });

        var line = new THREE.Line(lineGeometry, lineMaterial);
        line.name = 'labelLine'
        //console.log(line)
        this.scene.add(line);
    }

}