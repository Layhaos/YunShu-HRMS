import MyButton from "./MyButton.vue"

const components = {
    MyButton
};

export default {
    install(app) {
        for (const [key, component] of Object.entries(components)) {
            app.component(key, component);
        }
    }
};