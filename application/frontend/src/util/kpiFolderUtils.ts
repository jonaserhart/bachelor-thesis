import { AnalysisModel, KPI, KPIFolder } from '../features/analysis/types';

type TreeItem = {
  id: string;
  name: string;
  children: TreeItem[];
  kpis?: KPI[];
  icon?: React.ReactNode;
  isKPI?: boolean;
  isLeaf?: boolean;
};

export const isLeafNode = (
  value: string,
  data: (KPI | KPIFolder)[]
): boolean | undefined => {
  for (const node of data) {
    if (node.id === value) {
      return 'expression' in node;
    }
  }
  return undefined;
};

export const mapToFolderStructure = (
  kpiOrFolder: KPI | KPIFolder
): TreeItem => {
  if ('subFolders' in kpiOrFolder) {
    return {
      id: kpiOrFolder.id,
      name: kpiOrFolder.name,
      kpis: kpiOrFolder.kpis,
      children: [
        ...kpiOrFolder.subFolders.map(mapToFolderStructure),
        ...kpiOrFolder.kpis.map(mapToFolderStructure),
      ],
    };
  } else {
    return {
      id: kpiOrFolder.id,
      name: kpiOrFolder.name,
      isKPI: true,
      isLeaf: true,
      children: [],
    };
  }
};

export function getAllKPIs(model: AnalysisModel): KPI[] {
  let allKPIs: KPI[] = [...model.kpis]; // Initialize with root-level KPIs

  // Helper function to recursively collect KPIs from folders
  const collectKPIsFromFolder = (folder: KPIFolder) => {
    allKPIs = allKPIs.concat(folder.kpis); // Add KPIs from the current folder
    folder.subFolders.forEach((subFolder) => collectKPIsFromFolder(subFolder)); // Recur for each sub-folder
  };

  // Collect KPIs from all root-level folders
  model.kpiFolders.forEach((folder) => collectKPIsFromFolder(folder));

  return allKPIs;
}

export function getAllKPIsInFolder(
  model?: AnalysisModel,
  folderId?: string
): KPI[] {
  if (!folderId || !model) {
    return model?.kpis ?? [];
  }

  const collectKPIsFromFolder = (folder: KPIFolder): KPI[] => {
    if (folder.id === folderId) {
      return folder.kpis;
    }
    for (let subFolder of folder.subFolders) {
      const foundKPIs = collectKPIsFromFolder(subFolder);
      if (foundKPIs.length) {
        return foundKPIs;
      }
    }
    return [];
  };

  for (let folder of model.kpiFolders) {
    const found = collectKPIsFromFolder(folder);
    if (found.length) {
      return found;
    }
  }

  return [];
}

export function updateKPIFolderInModel(
  model: AnalysisModel,
  folderId: string,
  updateFunction: (folder: KPIFolder) => void
): void {
  for (const folder of model.kpiFolders) {
    if (folder.id === folderId) {
      updateFunction(folder);
      return;
    }
    const foundFolder = findFolderInFolder(folder, folderId, updateFunction);
    if (foundFolder) {
      return;
    }
  }
}

export function updateKPIParentFolderInModel(
  model: AnalysisModel,
  id: string,
  updateFunction: (folder: KPIFolder) => void
): void {
  for (const folder of model.kpiFolders) {
    if (folder.id === id) {
      updateFunction(folder);
      return;
    }
    const foundFolder = findParentFolder(folder, id, updateFunction);
    if (foundFolder) {
      return;
    }
  }
}

function findFolderInFolder(
  parentFolder: KPIFolder,
  folderId: string,
  updateFunction: (folder: KPIFolder) => void = () => {}
): KPIFolder | undefined {
  for (const folder of parentFolder.subFolders) {
    if (folder.id === folderId) {
      updateFunction(folder);
      return folder;
    }
    const foundFolder = findFolderInFolder(folder, folderId, updateFunction);
    if (foundFolder) {
      return foundFolder;
    }
  }
  return undefined;
}

function findParentFolder(
  parentFolder: KPIFolder,
  objId: string,
  updateFunction: (folder: KPIFolder) => void = () => {}
): KPIFolder | undefined {
  for (const folderOrKPI of [
    ...parentFolder.subFolders,
    ...parentFolder.kpis,
  ]) {
    if (folderOrKPI.id === objId) {
      // update parentfolder instead of direct folder or KPI
      updateFunction(parentFolder);
      return parentFolder;
    }
    if ('subFolders' in folderOrKPI) {
      const foundFolder = findParentFolder(folderOrKPI, objId, updateFunction);
      if (foundFolder) {
        return foundFolder;
      }
    }
  }
  return undefined;
}

export function updateKPIInModel(
  model: AnalysisModel,
  kpiId: string,
  updateFunction: (kpi: KPI) => void
): void {
  for (const kpi of model.kpis) {
    if (kpi.id === kpiId) {
      updateFunction(kpi);
      return;
    }
  }

  for (const folder of model.kpiFolders) {
    const foundKPI = findKPIInFolder(folder, kpiId, updateFunction);
    if (foundKPI) {
      return;
    }
  }
}

export function findKPIInFolder(
  folder: KPIFolder,
  kpiId: string,
  updateFunction: (kpi: KPI) => void = () => {}
): KPI | undefined {
  for (const kpi of folder.kpis) {
    if (kpi.id === kpiId) {
      updateFunction(kpi);
      return kpi;
    }
  }

  for (const subfolder of folder.subFolders) {
    const foundKPI = findKPIInFolder(subfolder, kpiId, updateFunction);
    if (foundKPI) {
      return foundKPI;
    }
  }

  return undefined;
}

export function moveKPIBetweenFoldersOrToModel(
  model: AnalysisModel,
  kpiId: string,
  to: string | undefined
) {
  let folderFrom: KPIFolder | undefined;
  for (let kpiFolder of model.kpiFolders) {
    folderFrom = findParentFolder(kpiFolder, kpiId);
    if (folderFrom !== undefined) {
      break;
    }
  }
  if (!folderFrom && !to) {
    throw new Error("Please define either 'from' or 'to'.");
  }

  // CASE: copy from root to a folder
  if (!folderFrom) {
    // assume kpi is on the model
    const kpiIndex = model.kpis.findIndex((x) => x.id === kpiId);
    if (kpiIndex < 0) {
      throw new Error(`KPI with id ${kpiId} was not found on model ${model}`);
    }

    // add kpi to destination folder
    updateKPIFolderInModel(model, to!, (f) =>
      f.kpis.push({ ...model.kpis[kpiIndex] })
    );
    // remove kpi from model
    model.kpis = model.kpis.filter((x) => x.id !== kpiId);
    return;
  }

  // CASE: copy from folder to root
  if (!to) {
    // assume we need to put kpi onto the model

    if (!folderFrom) {
      throw new Error('KPIFolder not found');
    }

    const kpi = folderFrom?.kpis.find((x) => x.id === kpiId);
    if (!kpi) {
      throw new Error('KPI not found');
    }

    model.kpis.push({ ...kpi });
    updateKPIFolderInModel(
      model,
      folderFrom.id,
      (f) => (f.kpis = f.kpis.filter((x) => x.id !== kpiId))
    );

    return;
  }

  // CASE: copy from folder to folder

  if (!folderFrom) {
    throw new Error('KPIFolder not found');
  }

  const kpi = folderFrom?.kpis.find((x) => x.id === kpiId);
  if (!kpi) {
    throw new Error('KPI not found');
  }

  updateKPIFolderInModel(model, to!, (folder) => folder.kpis.push({ ...kpi }));
  updateKPIFolderInModel(
    model,
    folderFrom.id,
    (folder) => (folder.kpis = folder.kpis.filter((x) => x.id !== kpiId))
  );
}
