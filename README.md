- [ENGLISH VERSION](#english-version)
- [ITALIAN VERSION](#italian-version)

# ENGLISH VERSION
# Smart Car 🚗

Project developed as part of the Information Visualization course at the University of Salerno.

## 👥 Team

- [Simone Scermino](https://github.com/Hikki00)
- [Ciro Vitale](https://github.com/cirovitale)

## 📝 Description

The project addresses Smart Mobility challenges, focusing on safe driving and compliance with road signs, both horizontal and vertical, through a Unity simulation enhanced by Reinforcement Learning techniques.

### Objectives

- Creation of a realistic simulation environment in Unity
- Implementation of an autonomous agent with environmental perception capabilities
- Agent training through Reinforcement Learning
- Testing the agent in diverse scenarios to evaluate its generalization capability

## 🛠️ Technologies Used

- Unity
- ML-Agents
- TensorBoard

## 🚀 Getting Started

### Demo

1. Download the project
2. Open the `smart-car` folder in Unity
3. Press Play to start the simulation with 5 karts using the best model (available in the `results` folder)

### Training and Monitoring

1. Create a Python virtual environment
2. Activate the virtual environment 
3. Install dependencies:

```bash
pip install -r requirements.txt
```

4. Run the training:

```bash
mlagents-learn config/trainer_config.yaml --run-id=run001
```

5. Monitor the training with TensorBoard:

```bash
tensorboard --logdir=results --port=6006
```

## 📊 Presentation

You can watch a demo video at the following link: [demo video](https://youtu.be/3ezWncyGiuk).

For more details on the project, please refer to the [complete presentation](https://docs.google.com/presentation/d/1Xg_qV4EM3N4ziOxmwtLusDE-ioAwfoSk/), for a correct viewing experience, view it in `presentation mode`.


# ITALIAN VERSION
# Smart Car 🚗

Progetto realizzato nell'ambito del corso di Information Visualization presso l'Università degli Studi di Salerno.

## 👥 Team

- [Simone Scermino](https://github.com/Hikki00)
- [Ciro Vitale](https://github.com/cirovitale)

## 📝 Descrizione

Il progetto affronta le sfide della Smart Mobility, concentrandosi sulla guida sicura e sul rispetto della segnaletica stradale, orizzontale e verticale, attraverso una simulazione in Unity potenziata da tecniche di Reinforcement Learning.

### Obiettivi

- Creazione di un ambiente di simulazione realistico in Unity
- Implementazione di un agente autonomo con capacità di percezione ambientale
- Training dell'agente mediante Reinforcement Learning
- Test dell'agente in scenari diversificati per valutarne la capacità di generalizzazione

## 🛠️ Tecnologie Utilizzate

- Unity
- ML-Agents
- TensorBoard

## 🚀 Come Iniziare

### Demo

1. Scaricare il progetto
2. Aprire la cartella `smart-car` in Unity
3. Premere Play per avviare la simulazione con 5 kart che utilizzano il modello migliore (disponibile nella cartella `results`)

### Training e Monitoraggio

1. Creare un ambiente virtuale Python
2. Attivare l'ambiente virtuale
3. Installare le dipendenze:

```bash
pip install -r requirements.txt
```

4. Eseguire il training:

```bash
mlagents-learn config/trainer_config.yaml --run-id=run001
```

5. Monitorare l'addestramento con TensorBoard:

```bash
tensorboard --logdir=results --port=6006
```

## 📊 Presentazione

È possibile visionare una demo video al [seguente link](https://youtu.be/3ezWncyGiuk).

Per maggiori dettagli sul progetto consultare la [presentazione completa](https://docs.google.com/presentation/d/1Xg_qV4EM3N4ziOxmwtLusDE-ioAwfoSk/), per una corretta fruizione visionare in `modalità presentazione`.
