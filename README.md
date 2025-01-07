# 3D Heatmap Data Visualization
![demo](https://github.com/LostVirgins/DataAnalysis_D3/blob/main/Dist/Demo.png)

## Overview

This repository provides tools for creating and visualizing 3D heatmaps from event data in a Unity environment. This system can be used to map and analyze in-game events such as player positions, deaths, or attacks by visualizing them in a 3D space using particle systems.

## Features
- Send and request event data to database.
- Load event data from external files.
- Visualize events in a 3D space using customizable particle systems.
- Flexible gradient-based heatmap color schemes.
- Supports filtering by specific event types (e.g., positions, attacks).
- Advanced settings to fine-tune visualization parameters like height, color cutoff, and particle density.

## Scene Heatmap Section
- **Heatmap Controller**: HeatmapController script attached.
- **Send Data To Server**: Server script attached. Disabled by default. Enable this gameobject to start recording gameplay data.
- **Retrieve Server Data**: AccessServerData script attached. Used by the Heatmap Controller.

## Heatmap Controller Script
The **Heatmap Controller** script is the main component responsible for managing and rendering the heatmap. It offers the following functionalities:

### **Actions**
1. **Load events from file**: Requests data from MySQL, saves it to the specified file, and loads it.
2. **Initialize particle system**: Prepares the particle system for visualization by configuring it with user-defined settings.
3. **Generate heatmap**: Processes the event data and generates the heatmap visualization.
4. **Reset heatmap values**: Clears existing heatmap data and resets the visualization.

### **Event Selection**
Allows users to choose which types of events to visualize, options are created dynamically depending on the Event types found. Demo data includes:
  - `Attack`
  - `Position`
  - `Death`
  - `Damaged`

Check the boxes to include these events in the heatmap visualization. For instance, selecting **Position** generates a heatmap based on player positions.

### **Settings**
These options configure the particle system used for visualization:
- **Particle Distance**: Controls the spacing between individual particles in the system _performance warning :)_.
- **Particle Size**: Defines the size of each particle.
- **Color Multiplier**: Adjusts the intensity of the heatmap colors.
- **Color Distance**: Modifies how far the event position affects particle color.
- **Gradient**: A color gradient used to represent the heatmap intensity visually.

### **Advanced Settings**
For finer control over the heatmap:
- **Color Cutoff**: Sets the threshold for the minimum intensity to display particles.
- **Particle System Height**: Adjusts the vertical height of the particle system (in particles).
- **Ignore Height (2D)**: Toggles 2D mode, ignoring height variations.

### **Event Data File**
Specifies the file containing event data. Example file: `game_server`.

### **Particle Material**
Defines the material used for rendering particles. The default is `Default-ParticleSystem`.

---

## Getting Started

### Prerequisites
- Unity 2021.3 or later
- Basic knowledge of Unity particle systems

### Installation
1. Clone this repository:
   ```bash
   git clone https://github.com/your-username/3D-Heatmap-Visualization.git
