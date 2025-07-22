function clearDoms(element) {
    while (element.firstChild) {
        element.removeChild(element.firstChild); // �Ƴ���һ���ӽڵ�

    }
    for (var i = element.attributes.length - 1; i >= 0; i--) {
        var attribute = element.attributes[i];
        if (attribute.name !== "class" && attribute.name !== "id") {
            element.removeAttribute(attribute.name);
        }
    }
}

function TableScroller(tableContainer, interval) {
    //console.log('table scroller')
    // ��Ӧ����¼�
    let that = this;
    tableContainer.on('mouseover', function () {

        that.pause();
    });
    tableContainer.on('mouseleave', function () {
        that.resume();
    });

    // ���ر�������
    let bodyContainer = tableContainer.find('.layui-table-body');
    //console.log(bodyContainer)
    bodyContainer.css('overflow-x', 'hidden');
    bodyContainer.css('overflow-y', 'hidden');

    this.timerID = null;
    this.interval = interval;
    this._bodyTable = bodyContainer.find('table');
    this._tbody = this._bodyTable.find('tbody');

    this.start = function () {
        let that = this;
        that.timerID = setInterval(function () {
            that._scroll(that._bodyTable, that._tbody, that.interval);
        }, that.interval);
    };
    this.pause = function () {
        let that = this;
        if (that.timerID === null) {
            return;
        }

        clearInterval(that.timerID);
        that.timerID = null;
    };
    this.resume = function () {
        let that = this;
        if (that.timerID !== null || that.callback === null || that.interval === null) {
            return;
        }

        that.timerID = setInterval(function () {
            that._scroll(that._bodyTable, that._tbody, that.interval);
        }, that.interval);
    };
    this.stop = function () {
        let that = this;
        if (this.timerID === null) {
            return;
        }

        clearInterval(that.timerID);
        that.callback = null;
        that.interval = null;
        that.timerID = null;
    };
    this._scroll = function (bodyTable, tbody, interval) {
        let firstRow = tbody.find('tr:first');
        let rowHeight = firstRow.height();
        bodyTable.animate({ top: '-' + rowHeight + 'px' }, interval * 0.5, function () {
            tbody.append(firstRow.prop("outerHTML"));
            bodyTable.css('top', '0px');
            firstRow.remove();
        });
    }
}

//���������Ƴ�����
function removeObj(objs, object) {
    const index = objs.indexOf(object);
    //console.log(object)
    //console.log(alarmObjs)
    //console.log('index= ' + index)
    if (index != -1) {
        objs.splice(Object)
    }

}

function getDate() {
    // ����Data����
    var date = new Date();
    // ��ȡ���
    var year = date.getFullYear();
    // ��ȡ�·ݣ�0-11��
    var month = date.getMonth() + 1;
    // ��ȡ���ڣ�1-31��
    var day31 = date.getDate();

    // ���·ݽ��д���1-9����ǰ�����һ����0��
    if (month >= 1 && month <= 9) {
        month = "0" + month;
    }
    //�����ڽ��д���1-9����ǰ�����һ����0��
    if (day31 >= 0 && day31 <= 9) {
        day31 = "0" + day31;
    }

    var separator = "-"; //��ӷָ�����-��
    var strDate = year + separator + month + separator + day31;

    return strDate;
}

function getSelectEQPname() {
    const infoBox = document.getElementById('map-info')
    const locationBox = document.getElementById('map-location')
    infoBox.innerHTML = window.eqid;
    locationBox.innerHTML = '-Location: ZJ PD7 AE';
}

function showStatusCount(countByCategory) {
    const divRun = document.getElementById('Run');
    const divAlarm = document.getElementById('Alarm');
    const divIdle = document.getElementById('Idle');
    const divDown = document.getElementById('Down');
    divRun.innerHTML = 0;
    divAlarm.innerHTML = 0;
    divIdle.innerHTML = 0;
    divDown.innerHTML = 0;
    countByCategory.forEach((value, key) => {
        const div = document.getElementById(key);
        if (div) { div.innerHTML = value; }
    })
}
function switchView() {
    const div1 = document.getElementById('mfg-data')
    const div2 = document.getElementById('history-alarm-time')
    const div3 = document.getElementById('history-alarm-rate')

    clearDoms(div1)
    clearDoms(div2)
    clearDoms(div3)

}

export { getDate, clearDoms, removeObj, getSelectEQPname, showStatusCount, switchView }