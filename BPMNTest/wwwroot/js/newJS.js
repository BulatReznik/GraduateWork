import BpmnModeler from 'bpmn-js/lib/Modeler';
import resizeTask from 'bpmn-js-task-resize/lib';

const containerId = '#canvas';

const modeler = new BpmnModeler({
    container: containerId, // Убедитесь, что модельер прикрепляется к правильному контейнеру
    additionalModules: [
        resizeTask
    ],
    taskResizingEnabled: true,
});

// Проверка после инициализации модельера
document.addEventListener('DOMContentLoaded', () => {
    const containerElement = document.querySelector(containerId);

    if (containerElement) {
        console.log('Контейнер найден:', containerElement);
        console.log('Модельер инициализирован:', modeler);
        console.log('Содержимое контейнера:', containerElement.innerHTML);
    } else {
        console.error('Контейнер не найден:', containerId);
    }

    // Привязка функции к кнопке
    window.exportBpmnXml = exportBpmnXml; // Сделать функцию доступной в глобальном контексте
    window.exportBpmnXmlForExecute = exportBpmnXmlForExecute;
});

// Функция для загрузки диаграммы
function loadDiagram(bpmnXml) {
    console.log(bpmnXml);
    modeler.importXML(bpmnXml).catch(err => {
        console.error('Ошибка при импорте XML:', err);
    });
}

// Функция для загрузки диаграммы из файла
function loadDiagramFromFile() {
    const canvasElement = document.getElementById('canvas');
    const diagramId = canvasElement.getAttribute('data-diagram-id');

    // Путь к файлу с XML-кодом диаграммы
    const bpmnBaseFile = `/UpdateDiagram?handler=Diagram&diagramId=${diagramId}`;
    console.log('Запрашиваемый URL:', bpmnBaseFile);

    // Чтение содержимого файла
    fetch(bpmnBaseFile)
        .then(response => {
            if (!response.ok) {
                throw new Error(`Ошибка HTTP: ${response.status}`);
            }
            return response.text();
        })
        .then(bpmnXml => {
            console.log('Загружаемый XML:', bpmnXml);
            loadDiagram(bpmnXml);
            deleteTempFile(diagramId);
        })
        .catch(error => {
            console.error('Не удалось загрузить BPMN файл:', error);
        });
}

function deleteTempFile(diagramId) {
    console.log(`/UpdateDiagram?handler=DeleteTempFile&diagramId=${diagramId}`);
    fetch(`/UpdateDiagram?handler=DeleteTempFile&diagramId=${diagramId}`)
        .then(response => {
            if (response.ok) {
                console.log('Временный файл успешно удален.');
            } else {
                console.error('Не удалось удалить временный файл.');
            }
        })
        .catch(error => {
            console.error('Ошибка при удалении временного файла:', error);
        });
}

async function exportBpmnXml() {
    const { xml } = await modeler.saveXML({ format: true });

    // Установите значение xml в скрытое поле формы
    const xmlInput = document.getElementById('XMLDiagram');
    xmlInput.value = xml;

    // Нажмите кнопку отправки формы
    const saveButton = document.getElementById('saveButton');
    saveButton.click();
}

async function exportBpmnXmlForExecute() {
    const { xml } = await modeler.saveXML({ format: true });

    // Установите значение xml в скрытое поле формы
    const xmlInput = document.getElementById('ExecuteXMLDiagram');
    xmlInput.value = xml;

    // Нажмите кнопку отправки формы
    const executeButton = document.getElementById('executeButton');
    executeButton.click();
}


// Вызываем функцию загрузки диаграммы из файла при загрузке страницы
document.addEventListener('DOMContentLoaded', (event) => {
    loadDiagramFromFile();
});
