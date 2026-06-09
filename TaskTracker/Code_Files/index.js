// Application state
let tasks = [];

// Fetch all DOM elements needed
const taskNameInput = document.getElementById("taskNameInput");
const taskDescriptionInput = document.getElementById("taskDescriptionInput");
const addTaskBtn = document.getElementById("addTaskBtn");
const taskList = document.getElementById("taskList");
const totalTasksSpan = document.getElementById("totalTasks");
const completedTasksSpan = document.getElementById("completedTasks");

// Task Model
class Task {
    constructor(name, description) {
        this.id = Date.now();
        this.name = name;
        this.description = description;
        this.completed = false;
        this.createdOn = new Date()
            .toLocaleString("en-IN", {
                weekday: "long",
                year: "numeric",
                month: "long",
                day: "numeric",
                hour: "numeric",
                minute: "2-digit",
                second: "2-digit"
            });
        ;
    }
}

function saveTasks() {
    localStorage.setItem("tasks", JSON.stringify(tasks));
}

function loadTasks() {
    const storedTasks = localStorage.getItem("tasks");

    if (storedTasks) {
        tasks = JSON.parse(storedTasks);
    }
}

function updateTaskCount() {
    const totalTasks = taskList.children.length;

    // Completed tasks = number of ticked checkboxes
    const completedTasks = taskList.querySelectorAll("input[type=checkbox]:checked").length;

    // update spans with the respective values
    totalTasksSpan.textContent = totalTasks;
    completedTasksSpan.textContent = completedTasks;
}

function renderTasks() {
    // Clear everything
    taskList.innerHTML = "";

    // Render every task
    tasks.forEach((task) => {
        const li = document.createElement("li");

        // Checkbox
        const checkbox = document.createElement("input");
        checkbox.type = "checkbox";
        checkbox.checked = task.completed;

        // Task name
        const nameSpan = document.createElement("span");
        nameSpan.textContent = task.name;
        nameSpan.classList.add("taskName");

        // Task Description
        const descriptionSpan = document.createElement("span");
        descriptionSpan.textContent = task.description;
        descriptionSpan.classList.add("taskDescription");

        // Creation Date
        const dateInfo = document.createElement("span");
        dateInfo.textContent = task.createdOn;
        dateInfo.classList.add("taskDate");

        // Overall Li Text
        const taskText = document.createElement("p");
        taskText.innerHTML = `${nameSpan.outerHTML}<br>(${dateInfo.outerHTML})<br>${descriptionSpan.outerHTML}`;
        taskText.classList.add("taskContent");

        if (task.completed) {
            taskText.classList.add("completed");
        }


        // Delete button
        const deleteButton = document.createElement("button");
        deleteButton.textContent = "Delete Task";
        deleteButton.classList.add("delbtn");

        // Events
        // Toggle completion
        checkbox.addEventListener("change", () => {
            task.completed = checkbox.checked;

            saveTasks();
            renderTasks();
        });

        // Delete task
        deleteButton.addEventListener("click", () => {
            tasks = tasks.filter((t) => t.id !== task.id);

            saveTasks();
            renderTasks();
        });

        // Append elements
        li.appendChild(checkbox);
        li.appendChild(taskText);
        li.appendChild(deleteButton);

        taskList.appendChild(li);
    });

    updateTaskCount();
}

function addTask() {
    const taskNameText = taskNameInput.value.trim();
    if (taskNameText === "") return;

    const taskDescriptionText = taskDescriptionInput.value.trim();
    const newTask = new Task(taskNameText, taskDescriptionText);

    tasks.push(newTask);

    saveTasks();
    renderTasks();

    taskNameInput.value = "";
    taskDescriptionInput.value = "";
}

function init() {
    loadTasks();
    renderTasks();

    addTaskBtn.addEventListener("click", addTask);
}
init();