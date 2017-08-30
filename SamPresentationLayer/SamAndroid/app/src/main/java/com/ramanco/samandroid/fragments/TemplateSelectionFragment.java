package com.ramanco.samandroid.fragments;


import android.app.ProgressDialog;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.content.ContextCompat;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;

import com.ramanco.samandroid.R;
import com.ramanco.samandroid.adapters.TemplateAdapter;
import com.ramanco.samandroid.api.dtos.TemplateCategoryDto;
import com.ramanco.samandroid.api.dtos.TemplateDto;
import com.ramanco.samandroid.api.endpoints.TemplatesApiEndpoint;
import com.ramanco.samandroid.exceptions.CallServerException;
import com.ramanco.samandroid.objects.StickyHeader;
import com.ramanco.samandroid.utils.ApiUtil;
import com.ramanco.samandroid.utils.ExceptionManager;
import com.ramanco.samandroid.utils.UxUtil;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import retrofit2.Response;
import se.emilsjolander.stickylistheaders.StickyListHeadersListView;

public class TemplateSelectionFragment extends Fragment {

    //region Ctors:
    public TemplateSelectionFragment() {
    }
    //endregion

    //region Fields:
    ProgressDialog progress;
    SendConsolationFragment parentView;
    //endregion

    //region Overrides:
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        View fragmentView = inflater.inflate(R.layout.fragment_template_selection, container, false);

        try {
            loadTemplatesAsync();

            //region list item click:
            StickyListHeadersListView listView = (StickyListHeadersListView) fragmentView.findViewById(R.id.lv_items);
            listView.setOnItemClickListener(new AdapterView.OnItemClickListener() {
                @Override
                public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                    try {
                        TemplateDto template = (TemplateDto) parent.getItemAtPosition(position);
                        parentView.setSelectedTemplate(template);
                        parentView.showTemplateFieldsStep();
                    } catch (Exception ex) {
                        ExceptionManager.handle(getActivity(), ex);
                    }
                }
            });
            //endregion

            //region nav buttons:
            parentView.setNextVisible(false);
            parentView.setPrevVisible(true);
            parentView.setOnPreviousClickListener(new Runnable() {
                @Override
                public void run() {
                    try {
                        parentView.showObitSelectionStep();
                    } catch (Exception ex) {
                        ExceptionManager.handle(getActivity(), ex);
                    }
                }
            });
            //endregion
        } catch (Exception ex) {
            ExceptionManager.handle(getActivity(), ex);
        }

        return fragmentView;
    }
    //endregion

    //region Getters & Setters:
    public SendConsolationFragment getParentView() {
        return parentView;
    }

    public void setParentView(SendConsolationFragment parentView) {
        this.parentView = parentView;
    }
    //endregion

    //region Methods:
    private void loadTemplatesAsync() {
        progress = UxUtil.showProgress(getActivity());
        new Thread(new Runnable() {
            @Override
            public void run() {
                try {
                    TemplatesApiEndpoint endpoint = ApiUtil.createEndpoint(TemplatesApiEndpoint.class);
                    Response<TemplateDto[]> response = endpoint.all().execute();
                    if (!response.isSuccessful())
                        throw new CallServerException(getActivity());
                    final TemplateDto[] templates = response.body();
                    //region fill list view:
                    getActivity().runOnUiThread(new Runnable() {
                        @Override
                        public void run() {
                            try {
                                fillListView(templates);
                                progress.dismiss();
                            } catch (Exception ex) {
                                progress.dismiss();
                                ExceptionManager.handle(getActivity(), ex);
                            }
                        }
                    });
                    //endregion
                } catch (Exception ex) {
                    progress.dismiss();
                    ExceptionManager.handle(getActivity(), ex);
                }
            }
        }).start();
    }

    private void fillListView(TemplateDto[] templates) {
        View fragmentView = getView();
        if (fragmentView != null) {
            // headers:
            List<StickyHeader> stickyHeaders = new ArrayList<>();
            List<TemplateCategoryDto> categories = getCategories(templates);

            Map<Integer, Integer> categorySizeMap = getCategorySizeMap(templates, categories);
            for (int i = 0; i < categories.size(); i++) {
                TemplateCategoryDto category = categories.get(i);
                int index = i == 0 ? 0 : sumOfSize(categories, categorySizeMap, i);
                StickyHeader stickyHeader = new StickyHeader(category.getName(), index,
                        ContextCompat.getColor(getActivity(), R.color.colorStickyListHeader),
                        ContextCompat.getColor(getActivity(), R.color.colorStickyListHeaderContrast));
                stickyHeaders.add(stickyHeader);
            }
            // items:
            StickyListHeadersListView lvItems = (StickyListHeadersListView) fragmentView.findViewById(R.id.lv_items);
            TemplateAdapter adapter = new TemplateAdapter(getActivity(), sortTemplates(categories, templates), stickyHeaders);
            lvItems.setAdapter(adapter);
        }
    }

    private List<TemplateCategoryDto> getCategories(TemplateDto[] templates) {
        List<Integer> tempList = new ArrayList<>();
        List<TemplateCategoryDto> list = new ArrayList<>();
        for (TemplateDto t : templates) {
            if (!tempList.contains(t.getCategory().getId())) {
                tempList.add(t.getCategory().getId());
                list.add(t.getCategory());
            }
        }

        Collections.sort(list, new Comparator<TemplateCategoryDto>() {
            @Override
            public int compare(TemplateCategoryDto o1, TemplateCategoryDto o2) {
                if (o1.getOrder() == o2.getOrder())
                    return 0;
                return o1.getOrder() < o2.getOrder() ? -1 : 1;
            }
        });

        return list;
    }

    private int getCategorySize(TemplateDto[] templates, int categoryId) {
        int count = 0;
        for (TemplateDto t : templates) {
            if (t.getTemplateCategoryID() == categoryId)
                count++;
        }
        return count;
    }

    private Map<Integer, Integer> getCategorySizeMap(TemplateDto[] templates, List<TemplateCategoryDto> categories) {
        Map<Integer, Integer> map = new HashMap<>();
        for (TemplateCategoryDto cat : categories) {
            map.put(cat.getId(), getCategorySize(templates, cat.getId()));
        }
        return map;
    }

    private int sumOfSize(List<TemplateCategoryDto> cats, Map<Integer, Integer> map, int endIndex) {
        int sum = 0;
        for (int i = 0; i < endIndex; i++) {
            sum += map.get(cats.get(i).getId());
        }
        return sum;
    }

    private TemplateDto[] sortTemplates(List<TemplateCategoryDto> cats, TemplateDto[] templates) {
        TemplateDto[] result = new TemplateDto[templates.length];
        int i = 0;
        for (TemplateCategoryDto cat : cats) {
            // retrieve cat templates:
            List<TemplateDto> tempList = new ArrayList<>();
            for (TemplateDto t : templates) {
                if (t.getTemplateCategoryID() == cat.getId())
                    tempList.add(t);
            }
            // sort templates by order:
            Collections.sort(tempList, new Comparator<TemplateDto>() {
                @Override
                public int compare(TemplateDto o1, TemplateDto o2) {
                    if (o1.getOrder() == o2.getOrder())
                        return 0;
                    return o1.getOrder() < o2.getOrder() ? -1 : 1;
                }
            });
            // add to result:
            for (TemplateDto t : tempList) {
                result[i] = t;
                i++;
            }
        }
        return result;
    }
    //endregion

}