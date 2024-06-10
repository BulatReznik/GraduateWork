import BpmnModeler from 'bpmn-js/lib/Modeler';
import resizeTask from 'bpmn-js-task-resize/lib';

window.modeler = new BpmnModeler({
    container: '#canvas',
    additionalModules: [
        resizeTask
    ],
    taskResizingEnabled: true,
    eventResizingEnabled: true
});

