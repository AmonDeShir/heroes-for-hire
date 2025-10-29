const plugin = require("tailwindcss/plugin")
const { theme, paths, plugins: onejsPlugins, corePlugins } = require('onejs-core/scripts/postcss/onejs-tw-config.cjs')

module.exports = {
  content: [...paths, "./@outputs/esbuild/**/*.js", "./comps/**/*.css"],
  theme: {
    ...theme,
    extend: {
      colors: {
        main: "#6B0D05",
        secondary: "#FDDB1D",
        secondaryDark: "#D7BA17",
        active: "#00B6D7",
        tertiary: "#E8E0B2",
        tertiaryLight: "#FFF7C6",
        textMain: "#6B0D05",
        textInverse: "#FFFFFF",
      }
    },
  },
  important: ".root",
  plugins: [
    ...onejsPlugins,
    plugin(function ({ addUtilities }) {
      addUtilities({
        ".default-bg-color": { "background-color": "white" },
        ".accented-bg-color": { "background-color": "#fde047" },
        ".hover-bg-color": { "background-color": "rgb(0 0 0 / 0.1)" },
        ".default-text-color": { "color": "#4b5563" },
        ".active-text-color": { "color": "#cd8c06" },
        ".highlighted-text-color": { "color": "#854d0e" },
      })
    }),
  ],
  corePlugins,
}
